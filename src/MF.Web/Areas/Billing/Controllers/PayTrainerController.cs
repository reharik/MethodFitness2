using System.Linq;
using System.Web.Mvc;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.ValidationServices;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Config;
using MF.Web.Controllers;
using NHibernate.Linq;
using MoreLinq;
using System.Collections.Generic;
using System;
using NHibernate.Transform;

namespace MF.Web.Areas.Billing.Controllers
{
    public class PayTrainerController:MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;

        public PayTrainerController(IRepository repository,
            ISaveEntityService saveEntityService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
        }

        public CustomJsonResult PayTrainer(PayTrainerViewModel input)
        {
            Notification notification;
            var trainer = _repository.Find<User>(input.EntityId);
            var trainerPayment = new TrainerPayment
            {
                Trainer = trainer,
                Total = input.paymentAmount
            };
            input.eligableRows.ForEach(x=>{

                var sessions = _repository.Query<Session>(s=>s.EntityId == x.id).Fetch(c=>c.Client);
                var session = sessions.FirstOrDefault();
                 session.TrainerPaid = true;
                trainerPayment.AddTrainerPaymentSessionItem(new TrainerPaymentSessionItem
                {
                    Appointment = session.Appointment,
                    Session = session,
                    AppointmentCost = session.Cost,
                    Client = session.Client,
                    TrainerPay = x.trainerPay
                });

                var crudManager = _saveEntityService.ProcessSave(session);
                crudManager.Finish();
            });
            
            if (trainerPayment == null)
            {
                notification = new Notification { Success = false, Message = WebLocalizationKeys.YOU_MUST_SELECT_AT_LEAST_ONE_SESSION.ToString() };
                return new CustomJsonResult(notification);
            } 
            trainer.AddTrainerPayment(trainerPayment);


            //var trainersQuery = _repository.Query<User>(x=>x.EntityId == input.EntityId).FetchMany(x=>x.Appointments).ThenFetchMany(x=>x.Clients).ThenFetchMany(x=>x.Sessions);
            //var trainer = trainersQuery.FirstOrDefault();
            //var trainerPayment = trainer.PayTrainer(input.eligableRows, input.paymentAmount);

            var crudManager2 = _saveEntityService.ProcessSave(trainer);
            var continuation = crudManager2.Finish();
            notification = new Notification(continuation);
            notification.Variable = UrlContext.GetUrlForAction<PayTrainerController>(x => x.TrainerReceipt(null),AreaName.Billing)+"/"+trainerPayment.EntityId+"?ParentId="+trainer.EntityId;
            return new CustomJsonResult(notification);
        }

        public ActionResult TrainerReceipt(ViewModel input)
        {
            var data = _repository.CurrentSession()
                .CreateSQLQuery("exec TrainerPaymentItems :TrainerPaymentId")
                        .SetResultTransformer(Transformers.AliasToBean<TrainerPaymentItems>())
                        .SetParameter("TrainerPaymentId", input.EntityId)
                        .List<TrainerPaymentItems>().ToList();
            
            var hourTotal = data.Count(x => x.AppointmentType == "Hour");
            var halfHourTotal = data.Count(x => x.AppointmentType == "Half Hour");
            var pairTotal = data.Count(x => x.AppointmentType == "Pair");
            var totalHours = hourTotal + pairTotal + (halfHourTotal+0.0m)/2;
            var model = new TrainerReceiptViewModel
            {
                TrainerName = data[0].TrainerName,
                Total = data[0].Total,
                CreatedDate= data[0].CreatedDate,
                SessionItems = data,
                hourTotal = hourTotal,
                halfHourTotal = halfHourTotal,
                pairTotal = pairTotal,
                totalHours = totalHours
            };
            return View(model);
        }
    }

    public class TrainerReceiptViewModel : ViewModel
    {
        public string TrainerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public IEnumerable<TrainerPaymentItems> SessionItems { get; set; }
        public double Total { get; set; }
        public int hourTotal { get; set; }
        public int halfHourTotal { get; set; }
        public int pairTotal { get; set; }
        public decimal totalHours { get; set; }
    }

    public class TrainerPaymentItems
    {
        public double Total { get; set; }
        public double AppointmentCost { get; set; }
        public double TrainerPay { get; set; }
        public string ClientName { get; set; }
        public string TrainerName { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AppointmentType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
