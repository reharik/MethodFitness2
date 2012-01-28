using System;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Rules;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Portfolio.Models.BulkAction;
using MethodFitness.Web.Areas.Schedule.Controllers;
using StructureMap;

namespace MethodFitness.Web.Controllers
{
    public class ClientController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;

        public ClientController(IRepository repository,
            ISaveEntityService saveEntityService, ISessionContext sessionContext)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            Client client;
            if (input.EntityId > 0)
            {
                client = _repository.Find<Client>(input.EntityId);
            }
            else
            {
                client = new Client {StartDate = DateTime.Now, SessionRates = new SessionRates(true)};
            }
            var model = new ClientViewModel
            {
                Item = client,
                Title = WebLocalizationKeys.CLIENT_INFORMATION.ToString(),
                DeleteUrl = UrlContext.GetUrlForAction<ClientController>(x=>x.Delete(null)),
                PaymentListUrl = UrlContext.GetUrlForAction<PaymentListController>(x=>x.ItemList(null),AreaName.Billing)+"?ParentId="+client.EntityId
            };
            return View(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            var client = _repository.Find<Client>(input.EntityId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteClientRules");
            var rulesResult = rulesEngineBase.ExecuteRules(client);
            if (!rulesResult.Success)
            {
                Notification notification = new Notification(rulesResult);
                return Json(notification,JsonRequestBehavior.AllowGet);
            }
            _repository.Delete(client);
            _repository.UnitOfWork.Commit();
            return Json(new Notification{Success = true}, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
            input.EntityIds.Each(x =>
            {
                var item = _repository.Find<Client>(x);
                _repository.Delete(item);
            });
            _repository.Commit();
            return Json(new Notification { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(ClientViewModel input)
        {
            Client client;
            client = input.EntityId > 0 ? _repository.Find<Client>(input.EntityId) : new Client();
            client = mapToDomain(input, client);
            associateWithUser(client);
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
            return Json(notification, "text/plain");
        }

        private void associateWithUser(Client client)
        {
            var userEntityId = _sessionContext.GetUserEntityId();
            var user = _repository.Find<User>(userEntityId);
            user.AddClient(client);
            _saveEntityService.ProcessSave(user);
        }

        private Client mapToDomain(ClientViewModel model, Client client)
        {
            var clientModel = model.Item;
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
            client.Status = clientModel.Status;
            client.BirthDate = clientModel.BirthDate;
            client.StartDate = clientModel.StartDate;
            if(client.SessionRates==null)client.SessionRates = new SessionRates();
            client.SessionRates.FullHour = clientModel.SessionRates.FullHour;
            client.SessionRates.HalfHour = clientModel.SessionRates.HalfHour;
            client.SessionRates.FullHourTenPack = clientModel.SessionRates.FullHourTenPack;
            client.SessionRates.HalfHourTenPack = clientModel.SessionRates.HalfHourTenPack;
            client.SessionRates.Pair = clientModel.SessionRates.Pair;
            return client;
        }
    }

    public class ClientViewModel:ViewModel
    {
        public Client Item { get; set; }
        public string DeleteUrl { get; set; }
        public string PaymentListUrl { get; set; }
    }
}