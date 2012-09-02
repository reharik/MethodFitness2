using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Rules;
using MethodFitness.Core.Services;
using MethodFitness.Security.Interfaces;
using MethodFitness.Web.Areas.Portfolio.Models.BulkAction;
using StructureMap;
using xVal.ServerSide;

namespace MethodFitness.Web.Controllers
{
    public class TrainerController : AdminController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly IFileHandlerService _uploadedFileHandlerService;
        private readonly ISessionContext _sessionContext;
        private readonly ISecurityDataService _securityDataService;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IUpdateCollectionService _updateCollectionService;

        public TrainerController(IRepository repository,
            ISaveEntityService saveEntityService,
            IFileHandlerService uploadedFileHandlerService,
            ISessionContext sessionContext,
            ISecurityDataService securityDataService,
            IAuthorizationRepository authorizationRepository,
            IUpdateCollectionService updateCollectionService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _uploadedFileHandlerService = uploadedFileHandlerService;
            _sessionContext = sessionContext;
            _securityDataService = securityDataService;
            _authorizationRepository = authorizationRepository;
            _updateCollectionService = updateCollectionService;
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            Trainer trainer;
//            if (input.EntityId > 0)
//            {
//                trainer = _repository.Find<Trainer>(input.EntityId);
//            }
//            else
//            {
//                trainer = new Trainer();
//                trainer.ClientRateDefault = Int32.Parse(SiteConfig.Settings().TrainerClientRateDefault);
//            }
//            var clients = _repository.FindAll<Client>();
//            var availableClients = clients.Select(x => new TCRTokenInputDto { id = x.EntityId, name = x.FullNameLNF, percentage = trainer.ClientRateDefault});
//            var selectedClients = trainer.Clients.Any()
//                ? trainer.Clients.Select(x => new TCRTokenInputDto { id = x.EntityId, name = x.FullNameLNF, percentage = trainer.TrainerClientRates.FirstOrDefault(c=>c.Client==x).Percent})
//                : null;
//            var userRoles = _repository.FindAll<UserRole>();
//            var availableUserRoles = userRoles.Select(x => new TokenInputDto { id = x.EntityId, name = x.Name});
//            var selectedUserRoles = trainer.UserRoles.Any()
//                ? trainer.UserRoles.Select(x => new TokenInputDto { id = x.EntityId, name = x.Name })
//                : null;
//
            var model = new TrainerViewModel();
//            {
//                Item = trainer,
//                DeleteUrl = UrlContext.GetUrlForAction<TrainerController>(x=>x.Delete(null)),
//                AvailableItems = availableUserRoles,
//                SelectedItems = selectedUserRoles,
//                AvailableClients = availableClients,
//                SelectedClients = selectedClients,
//                _Title = WebLocalizationKeys.TRAINER_INFORMATION.ToString()
//            };
            return PartialView("TrainerAddUpdate", model);
        }

        public ActionResult Display(ViewModel input)
        {
            var trainer = _repository.Find<Trainer>(input.EntityId);
            var model = new TrainerViewModel
            {
                Item = trainer,
                addUpdateUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.AddUpdate(null)) + "/" + trainer.EntityId,
                _Title = WebLocalizationKeys.TRAINER_INFORMATION.ToString()
            };
            return PartialView("TrainerView", model);
        }

        public ActionResult Delete(ViewModel input)
        {
            var trainer = _repository.Find<User>(input.EntityId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteTrainerRules");
            var validationManager = rulesEngineBase.ExecuteRules(trainer);
            if (validationManager.GetLastValidationReport().Success)
            {
                _repository.Delete(trainer);
            }
            var notification = validationManager.FinishWithAction();
            return Json(notification, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteTrainerRules");
            IValidationManager<User> validationManager = new ValidationManager<User>(_repository);
            input.EntityIds.ForEachItem(x =>
            {
                var item = _repository.Find<User>(x);
                validationManager = rulesEngineBase.ExecuteRules(item, validationManager);
                var report = validationManager.GetLastValidationReport();
                if (report.Success)
                {
                    report.SuccessAction = a => _repository.Delete(a);
                }
            });
            var notification = validationManager.FinishWithAction();
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(TrainerViewModel input)
        {
            Trainer trainer;
            trainer = input.EntityId > 0 ? _repository.Find<Trainer>(input.EntityId) : new Trainer();
            trainer = mapToDomain(input, trainer);
            ActionResult json;
            if (userRoleRules(trainer, out json)) return json;

            handlePassword(input, trainer);
            addSecurityUserGroups(trainer);
//            if (input.DeleteImage)
//            {
////                _uploadedFileHandlerService.DeleteFile(trainer.ImageUrl);
//                trainer.ImageUrl = string.Empty;
//            }
//            
//            var file = _uploadedFileHandlerService.RetrieveUploadedFile();
////            var serverDirectory = "/CustomerPhotos/" + _sessionContext.GetCompanyId() + "/Trainers";
//            trainer.ImageUrl = _uploadedFileHandlerService.GetUrlForFile(file, trainer.FirstName + "_" + trainer.LastName);
            var crudManager = _saveEntityService.ProcessSave(trainer);

//            _uploadedFileHandlerService.SaveUploadedFile(file, trainer.FirstName + "_" + trainer.LastName);
            var notification = crudManager.Finish();
            return Json(notification, "text/plain");
        }

        private bool userRoleRules(User trainer, out ActionResult json)
        {
            Notification notification;
            if (!trainer.UserRoles.Any())
            {
                notification = new Notification {Success = false};
                notification.Errors = new List<ErrorInfo>
                                          {
                                              new ErrorInfo(WebLocalizationKeys.USER_ROLES.ToString(),
                                                            WebLocalizationKeys.SELECT_AT_LEAST_ONE_USER_ROLE.ToString())
                                          };
                {
                    json = Json(notification, JsonRequestBehavior.AllowGet);
                    return true;
                }
            }
            if (trainer.UserRoles.FirstOrDefault(x => x.Name == "Trainer") == null)
            {
                notification = new Notification {Success = false};
                notification.Errors = new List<ErrorInfo>
                                          {
                                              new ErrorInfo(WebLocalizationKeys.USER_ROLES.ToString(),
                                                            WebLocalizationKeys.MUST_HAVE_TRAINER_USER_ROLE.ToString())
                                          };
                {
                    json = Json(notification, JsonRequestBehavior.AllowGet);
                    return true;
                }
            }
            if (!trainer.UserRoles.Any())
            {
                notification = new Notification {Success = false};
                notification.Errors = new List<ErrorInfo>
                                          {
                                              new ErrorInfo(WebLocalizationKeys.USER_ROLES.ToString(),
                                                            WebLocalizationKeys.SELECT_AT_LEAST_ONE_USER_ROLE.ToString())
                                          };
                {
                    json = Json(notification, JsonRequestBehavior.AllowGet);
                    return true;
                }
            }
            if (trainer.UserRoles.FirstOrDefault(x => x.Name == "Trainer") == null)
            {
                notification = new Notification {Success = false};
                notification.Errors = new List<ErrorInfo>
                                          {
                                              new ErrorInfo(WebLocalizationKeys.USER_ROLES.ToString(),
                                                            WebLocalizationKeys.MUST_HAVE_TRAINER_USER_ROLE.ToString())
                                          };
                {
                    json = Json(notification, JsonRequestBehavior.AllowGet);
                    return true;
                }
            }
            json = null;
            return false;
        }

        private void addSecurityUserGroups(User trainer)
        {
            _authorizationRepository.AssociateUserWith(trainer,SecurityUserGroups.Trainer.ToString());
            if(trainer.UserRoles.Any(x=>x.Name==SecurityUserGroups.Administrator.ToString()))
            {
                _authorizationRepository.AssociateUserWith(trainer, SecurityUserGroups.Administrator.ToString());
            }
        }

        private void handlePassword(TrainerViewModel input, User origional)
        {
            if (input.Password.IsNotEmpty())
            {
                origional.UserLoginInfo.Salt = _securityDataService.CreateSalt();
                origional.UserLoginInfo.Password = _securityDataService.CreatePasswordHash(input.Password,
                                                            origional.UserLoginInfo.Salt);
            }
        }

        private Trainer mapToDomain(TrainerViewModel model, Trainer trainer)
        {
            var trainerModel = model.Item;
            trainer.Address1 = trainerModel.Address1;
            trainer.Address2 = trainerModel.Address2;
            trainer.FirstName = trainerModel.FirstName;
            trainer.LastName = trainerModel.LastName;
            trainer.Email = trainerModel.Email;
            trainer.PhoneMobile = trainerModel.PhoneMobile;
            trainer.City = trainerModel.City;
            trainer.State = trainerModel.State;
            trainer.ZipCode = trainerModel.ZipCode;
            trainer.Notes = trainerModel.Notes;
            trainer.Status = trainerModel.Status;
            trainer.BirthDate = trainerModel.BirthDate;
            trainer.ClientRateDefault = trainerModel.ClientRateDefault;
            trainer.Color = trainerModel.Color.IsNotEmpty()?trainerModel.Color:"#3366CC";
            if (trainer.UserLoginInfo==null) trainer.UserLoginInfo = new UserLoginInfo();
            trainer.UserLoginInfo.LoginName = trainerModel.UserLoginInfo.LoginName;

            _updateCollectionService.UpdateFromCSV(trainer.UserRoles, model.UserRolesInput, trainer.AddUserRole, trainer.RemoveUserRole);
            updateClientInfo(model, trainer);
            return trainer;
        }
        private User updateClientInfo(TrainerViewModel model, Trainer trainer)
        {
//            var remove = new List<Client>();
//            if (model.SelectedClients == null)
//            {
//                trainer.Clients.ForEachItem(remove.Add);
//                remove.ForEachItem(trainer.RemoveClient);
//            }
//            else
//            {
//                model.SelectedClients.ForEachItem(x =>
//                                               {
//                                                   var client = _repository.Find<Client>(x.id);
//                                                   trainer.AddClient(client, x.percentage);
//                                               });
//                trainer.Clients.ForEachItem(x =>
//                                         {
//                                             if (!model.SelectedClients.Any(c => c.id == x.EntityId))
//                                                 trainer.RemoveClient(x);
//                                         });
//            }
            return trainer;
        }
    }

    public class TCRTokenInputDto : TokenInputDto
    {
        public int percentage { get; set; }
    }

    public class TrainerViewModel:ViewModel
    {
        public Trainer Item { get; set; }
        public IEnumerable<TokenInputDto> AvailableItems { get; set; }
        public IEnumerable<TokenInputDto> SelectedItems { get; set; }
        public bool DeleteImage { get; set; }
        public string Password { get; set; }
        [ValidateSameAs("Password")]
        public string PasswordConfirmation { get; set; }
        public string UserRolesInput { get; set; }
        public string ClientsInput { get; set; }

        public IEnumerable<TCRTokenInputDto> AvailableClients { get; set; }
        public IEnumerable<TCRTokenInputDto> SelectedClients { get; set; }

        public string DeleteUrl { get; set; }
    }
}