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
            var trainer = _repository.Query<User>(x => x.EntityId == input.ParentId)
                .FetchMany(x => x.TrainerPayments)
                .ThenFetchMany(x => x.TrainerPaymentSessionItems)
                .ThenFetch(x=>x.Appointment)    
                .ThenFetchMany(x=>x.Clients).ToList()
                .FirstOrDefault();
            var payment = trainer.TrainerPayments.FirstOrDefault(x => x.EntityId == input.EntityId);
            var sessionItems = payment
              .TrainerPaymentSessionItems
              .ToList().DistinctBy(x => x.Client.EntityId + x.Appointment.StartTime.ToString())
              .OrderBy(x => x.Client.LastName)
              .ThenBy(x => x.Appointment.Date);
            var model = new TrainerReceiptViewModel
                                              {
                                                  Trainer = trainer,
                                                  TrainerPayment = payment,
                                                  SessionItems = sessionItems 
                                              };
            return View(model);
        }
    }

    public class TrainerReceiptViewModel : ViewModel
    {
        public User Trainer { get; set; }
        public TrainerPayment TrainerPayment { get; set; }
        public IEnumerable<TrainerPaymentSessionItem> SessionItems { get; set; }
    }
}