using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CC.Core;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Enumerations;
using CC.Core.Html;
using CC.Core.Localization;
using CC.Core.Services;
using CC.Core.ValidationServices;
using Castle.Components.Validator;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Rules;
using MF.Core.Services;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Schedule.Models.BulkAction;
using MF.Web.Config;
using MethodFitness.Web;
using StructureMap;
using CC.Core.CustomAttributes;
using AreaName = MF.Core.Enumerations.AreaName;

namespace MF.Web.Controllers
{
    public class ClientController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;
        private readonly ISelectListItemService _selectListItemService;

        public ClientController(IRepository repository,
            ISaveEntityService saveEntityService, 
            ISessionContext sessionContext,
            ISelectListItemService selectListItemService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
            _selectListItemService = selectListItemService;
        }

        public ActionResult AddUpdate_Template(ViewModel input)
        {
            return View("AddUpdate", new ClientViewModel());
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            Client client;
            var baseSessionRate = _repository.Query<BaseSessionRate>().FirstOrDefault();
            if (input.EntityId > 0)
            {
                client = _repository.Find<Client>(input.EntityId);
                client.SessionRates.FullHour = client.SessionRates.FullHour > 0 ? client.SessionRates.FullHour : baseSessionRate.FullHour;
                client.SessionRates.HalfHour = client.SessionRates.HalfHour > 0 ? client.SessionRates.HalfHour : baseSessionRate.HalfHour;
                client.SessionRates.FullHourTenPack = client.SessionRates.FullHourTenPack > 0 ? client.SessionRates.FullHourTenPack : baseSessionRate.FullHourTenPack;
                client.SessionRates.HalfHourTenPack = client.SessionRates.HalfHourTenPack > 0 ? client.SessionRates.HalfHourTenPack : baseSessionRate.HalfHourTenPack;
                client.SessionRates.Pair = client.SessionRates.Pair > 0 ? client.SessionRates.Pair : baseSessionRate.Pair;
                client.SessionRates.PairTenPack = client.SessionRates.PairTenPack > 0 ? client.SessionRates.PairTenPack : baseSessionRate.PairTenPack;
            }
            else
            {
                client = new Client { StartDate = DateTime.Now, SessionRates = new SessionRates(baseSessionRate) };
            }
            //hijacking sessionratesdto since I need exact same object just different name
            var clientSessionsDto = new SessionRatesDto
            {
                FullHour = client.Sessions.Any(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears)
                    ? -client.Sessions.Count(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears)
                    : client.Sessions.Count(x => x.AppointmentType == AppointmentType.Hour.ToString()&&!x.SessionUsed),
                HalfHour = client.Sessions.Any(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears)
                    ? -client.Sessions.Count(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears)
                    : client.Sessions.Count(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && !x.SessionUsed),
                Pair = client.Sessions.Any(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears)
                    ? -client.Sessions.Count(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears)
                    : client.Sessions.Count(x => x.AppointmentType == AppointmentType.Pair.ToString() && !x.SessionUsed),
            };
            var model = Mapper.Map<Client,ClientViewModel>(client);

            model._Title = WebLocalizationKeys.CLIENT_INFORMATION.ToString();
            model._deleteUrl = UrlContext.GetUrlForAction<ClientController>(x=>x.Delete(null));
            model._paymentListUrl = UrlContext.GetUrlForAction<PaymentListController>(x=>x.ItemList(null),AreaName.Billing)+"?ParentId="+client.EntityId;
            model._sessionsAvailable = clientSessionsDto;
            model._saveUrl = UrlContext.GetUrlForAction<ClientController>(x => x.Save(null));
            model._StateList = _selectListItemService.CreateList<State>(); 
            model._SourceList = _selectListItemService.CreateList<Source>();
            return new CustomJsonResult(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            var client = _repository.Find<Client>(input.EntityId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteClientRules");
            var validationManager = rulesEngineBase.ExecuteRules(client);
            if (validationManager.GetLastValidationReport().Success)
            {
                _repository.Delete(client);
            }
            var notification = validationManager.Finish();
            return new CustomJsonResult(notification);

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteClientRules");
            IValidationManager validationManager = new ValidationManager(_repository);
            input.EntityIds.ForEachItem(x =>
            {
                var client = _repository.Find<Client>(x);
                validationManager = rulesEngineBase.ExecuteRules(client, validationManager);
                var report = validationManager.GetLastValidationReport();
                if (report.Success)
                {
                    report.SuccessAction = a => _repository.Delete((Client)a);
                }
            });
            var notification = validationManager.FinishWithAction();
            return new CustomJsonResult(notification);
        }

        public ActionResult Save(ClientViewModel input)
        {
            Client client;
            client = input.EntityId > 0 ? _repository.Find<Client>(input.EntityId) : new Client();
            client = mapToDomain(input, client);
            if (client.IsNew())
            {
                associateWithUser(client);
            }
//            if (input.DeleteImage)
//            {
////                _uploadedFileHandlerService.DeleteFile(client.ImageUrl);
//                client.ImageUrl = string.Empty;
//            }
//            
//            var file = _uploadedFileHandlerService.RetrieveUploadedFile();
////            var serverDirectory = "/CustomerPhotos/" + _sessionContext.GetCompanyId() + "/Clients";
//            client.ImageUrl = _uploadedFileHandlerService.GetUrlForFile(file, client.FirstName + "_" + client.LastName);
            var crudManager = _saveEntityService.ProcessSave(client);

//            _uploadedFileHandlerService.SaveUploadedFile(file, client.FirstName + "_" + client.LastName);
            var notification = crudManager.Finish();
            return new CustomJsonResult(notification){ ContentType = "text/plain" };
        }

        private void associateWithUser(Client client)
        {
            var userEntityId = _sessionContext.GetUserId();
            var trainer = _repository.Find<User>(userEntityId);
            trainer.AddClient(client, trainer.ClientRateDefault);
            _saveEntityService.ProcessSave(trainer);
        }

        private Client mapToDomain(ClientViewModel clientModel, Client client)
        {
            client.Address1 = clientModel.Address1;
            client.Address2 = clientModel.Address2;
            client.FirstName = clientModel.FirstName;
            client.LastName = clientModel.LastName;
            client.Email = clientModel.Email;
            client.MobilePhone = clientModel.MobilePhone;
            client.City = clientModel.City;
            client.State = clientModel.State;
            client.ZipCode = clientModel.ZipCode;
            client.Notes = clientModel.Notes;
            client.SourceOther = clientModel.SourceOther;
            client.Source = clientModel.Source;
            client.StartDate = clientModel.StartDate;
            client.SecondaryPhone = clientModel.SecondaryPhone;
            client.BirthDate = clientModel.BirthDate;
            if (client.SessionRates == null)
            {
                var baseSessionRate = _repository.Query<BaseSessionRate>().FirstOrDefault();
                client.SessionRates = new SessionRates(baseSessionRate);
            }
            if (clientModel.SessionRatesFullHour > 0) client.SessionRates.FullHour = clientModel.SessionRatesFullHour;
            if(clientModel.SessionRatesHalfHour>0) client.SessionRates.HalfHour = clientModel.SessionRatesHalfHour;
            if(clientModel.SessionRatesFullHourTenPack>0) client.SessionRates.FullHourTenPack = clientModel.SessionRatesFullHourTenPack;
            if(clientModel.SessionRatesHalfHourTenPack>0)client.SessionRates.HalfHourTenPack = clientModel.SessionRatesHalfHourTenPack;
            if(clientModel.SessionRatesPair>0) client.SessionRates.Pair = clientModel.SessionRatesPair;
            if(clientModel.SessionRatesPairTenPack>0)client.SessionRates.PairTenPack = clientModel.SessionRatesPairTenPack;
            return client;
        }
    }

    public class ClientViewModel:ViewModel
    {
        public string _deleteUrl { get; set; }
        public string _paymentListUrl { get; set; }
        [ValidateNonEmpty]
        public string FirstName { get; set; }
        [ValidateNonEmpty]
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [ValueOf(typeof(State))]
        public string State { get; set; }
        public IEnumerable<SelectListItem> _StateList { get; set; }
        public string ZipCode { get; set; }
        [ValidateNonEmpty]
        public string Email { get; set; }
        [ValidateNonEmpty]
        public string MobilePhone { get; set; }
        public string SecondaryPhone { get; set; }
        [ValueOf(typeof(Source))]
        public string Source { get; set; }
        public IEnumerable<SelectListItem> _SourceList { get; set; }

        [ValidateNonEmpty]
        public DateTime StartDate { get; set; }
        public string SourceOther { get; set; }
        [TextArea]
        public string Notes { get; set; }
        public double SessionRatesFullHour { get; set; }
        public double SessionRatesHalfHour { get; set; }
        public double SessionRatesFullHourTenPack { get; set; }
        public double SessionRatesHalfHourTenPack { get; set; }
        public double SessionRatesPair { get; set; }
        public double SessionRatesPairTenPack { get; set; }
        public SessionRatesDto _sessionsAvailable { get; set; }
    }
}