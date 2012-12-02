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
using CC.Core.Services;
using CC.Security.Interfaces;
using Castle.Components.Validator;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Rules;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Portfolio.Models.BulkAction;
using MethodFitness.Web.Config;
using StructureMap;
using xVal.ServerSide;

namespace MethodFitness.Web.Controllers
{
    public class TrainerController : AdminController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly IFileHandlerService _fileHandlerService;
        private readonly ISessionContext _sessionContext;
        private readonly ISecurityDataService _securityDataService;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IUpdateCollectionService _updateCollectionService;
        private readonly ISelectListItemService _selectListItemService;

        public TrainerController(IRepository repository,
            ISaveEntityService saveEntityService,
            IFileHandlerService fileHandlerService,
            ISessionContext sessionContext,
            ISecurityDataService securityDataService,
            IAuthorizationRepository authorizationRepository,
            IUpdateCollectionService updateCollectionService,
            ISelectListItemService selectListItemService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _fileHandlerService = fileHandlerService;
            _sessionContext = sessionContext;
            _securityDataService = securityDataService;
            _authorizationRepository = authorizationRepository;
            _updateCollectionService = updateCollectionService;
            _selectListItemService = selectListItemService;
        }

        public ActionResult AddUpdate_Template(ViewModel input)
        {
            return View("TrainerAddUpdate", new TrainerViewModel());
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            User trainer;
            if (input.EntityId > 0)
            {
                trainer = _repository.Find<User>(input.EntityId);
            }
            else
            {
                trainer = new User();
                trainer.ClientRateDefault = Int32.Parse(SiteConfig.Settings().TrainerClientRateDefault);
            }
            var clients = _repository.FindAll<Client>();
            var model = Mapper.Map<User, TrainerViewModel>(trainer);
            var _availableClients = clients.Select(x => new TCRTokenInputDto { id = x.EntityId.ToString(), name = x.FullNameLNF });
            var selectedClients = trainer.Clients.Select(x =>
                                                             {
                                                                 var tcr = trainer.TrainerClientRates.FirstOrDefault(c => c.Client == x);
                                                                 var percentage = tcr!=null ? tcr.Percent:trainer.ClientRateDefault;
                                                                 return new TCRTokenInputDto
                                                                             {
                                                                                 id = x.EntityId.ToString(),
                                                                                 name = x.FullNameLNF,
                                                                                 percentage = percentage
                                                                             };
                                                             });
            model.ClientsDtos = new TCRTokenInputViewModel { _availableItems = _availableClients, selectedItems = selectedClients.OrderBy(x=>x.name) };

            var userRoles = _repository.FindAll<UserRole>();
            var _availableUserRoles = userRoles.Select(x => new TokenInputDto { id = x.EntityId.ToString(), name = x.Name});
            var selectedUserRoles = trainer.UserRoles.Select(x => new TokenInputDto {id = x.EntityId.ToString(), name = x.Name});
            model.UserRolesDtos = new TokenInputViewModel { _availableItems = _availableUserRoles, selectedItems = selectedUserRoles };

            model._StateList = _selectListItemService.CreateList<State>();

            model._deleteUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.Delete(null));
            model._saveUrl= UrlContext.GetUrlForAction<TrainerController>(x => x.Save(null));
            model._Title = WebLocalizationKeys.TRAINER_INFORMATION.ToString();
            return new CustomJsonResult { Data = model };
        }

        public ActionResult Display_Template(ViewModel input)
        {
            return View("TrainerView", new TrainerViewModel());
        }
        public ActionResult Display(ViewModel input)
        {
            var trainer = _repository.Find<User>(input.EntityId);
            var model = Mapper.Map<User, TrainerViewModel>(trainer);
            model.addUpdateUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.AddUpdate(null)) + "/" + trainer.EntityId;
            model._Title = WebLocalizationKeys.TRAINER_INFORMATION.ToString();
            return new CustomJsonResult { Data = model };
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
            return new CustomJsonResult { Data = notification };

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteTrainerRules");
            IValidationManager validationManager = new ValidationManager(_repository);
            input.EntityIds.ForEachItem(x =>
            {
                var item = _repository.Find<User>(x);
                validationManager = rulesEngineBase.ExecuteRules(item, validationManager);
                var report = validationManager.GetLastValidationReport();
                if (report.Success)
                {
                    report.SuccessAction = a => _repository.Delete((User)a);
                }
            });
            var notification = validationManager.FinishWithAction();
            return new CustomJsonResult { Data = notification };
        }

        public ActionResult Save(TrainerViewModel input)
        {
            User trainer;
            trainer = input.EntityId > 0 ? _repository.Find<User>(input.EntityId) : new User();
            trainer = mapToDomain(input, trainer);
            ActionResult json;
            if (userRoleRules(trainer, out json)) return json;

            handlePassword(input, trainer);
            addSecurityUserGroups(trainer);
//            if (input.DeleteImage)
//            {
//                _fileHandlerService.DeleteFile(trainer.ImageUrl);
//                trainer.ImageUrl = string.Empty;
//            }
//            
//            var file = _fileHandlerService.RetrieveUploadedFile();
////            var serverDirectory = "/CustomerPhotos/" + _sessionContext.GetCompanyId() + "/Trainers";
//            trainer.ImageUrl = _fileHandlerService.GetUrlForFile(file, trainer.FirstName + "_" + trainer.LastName);
            var crudManager = _saveEntityService.ProcessSave(trainer);

//            _fileHandlerService.SaveUploadedFile(file, trainer.FirstName + "_" + trainer.LastName);
            var notification = crudManager.Finish();
            return new CustomJsonResult { Data = notification, ContentType = "text/plain" };
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
                    json = new CustomJsonResult { Data = notification };

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
                    json = new CustomJsonResult { Data = notification };
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
                    json = new CustomJsonResult { Data = notification };
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
                    json = new CustomJsonResult { Data = notification };
                    return true;
                }
            }
            json = null;
            return false;
        }

        private void addSecurityUserGroups(User trainer)
        {
            _authorizationRepository.AssociateUserWith(trainer,UserType.Trainer.ToString());
            if(trainer.UserRoles.Any(x=>x.Name==UserType.Administrator.ToString()))
            {
                _authorizationRepository.AssociateUserWith(trainer, UserType.Administrator.ToString());
            }else
            {
                _authorizationRepository.DetachUserFromGroup(trainer, UserType.Administrator.ToString());
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

        private User mapToDomain(TrainerViewModel model, User trainer)
        {
            trainer.Address1 = model.Address1;
            trainer.Address2 = model.Address2;
            trainer.FirstName = model.FirstName;
            trainer.LastName = model.LastName;
            trainer.Email = model.Email;
            trainer.PhoneMobile = model.PhoneMobile;
            trainer.City = model.City;
            trainer.State = model.State;
            trainer.ZipCode = model.ZipCode;
            trainer.BirthDate = model.BirthDate;
            trainer.ClientRateDefault = model.ClientRateDefault;
            trainer.Color = model.Color.IsNotEmpty() ? model.Color : "#3366CC";
            if (trainer.UserLoginInfo==null) trainer.UserLoginInfo = new UserLoginInfo();
            trainer.UserLoginInfo.LoginName = model.UserLoginInfoLoginName;
            
            _updateCollectionService.Update(trainer.UserRoles, model.UserRolesDtos, trainer.AddUserRole, trainer.RemoveUserRole);
            updateClientInfo(model, trainer);
            return trainer;
        }
        private User updateClientInfo(TrainerViewModel model, User trainer)
        {
            var remove = new List<Client>();
            if (model.ClientsDtos == null || model.ClientsDtos.selectedItems == null)
            {
                trainer.Clients.ForEachItem(remove.Add);
                remove.ForEachItem(trainer.RemoveClient);
            }
            else
            {
                model.ClientsDtos.selectedItems.ForEachItem(x =>
                                               {
                                                   var client = _repository.Find<Client>(Int32.Parse(x.id));
                                                   trainer.AddClient(client,0);
                                                   var tcr = trainer.TrainerClientRates.FirstOrDefault(r => r.Client == client);
                                                   if (tcr != null) { tcr.Percent = x.percentage; }
                                                   else{trainer.AddTrainerClientRate(new TrainerClientRate{Client = client,Percent = x.percentage,Trainer = trainer});}
                                               });
                trainer.Clients.ForEachItem(x =>
                                         {
                                             if (!model.ClientsDtos.selectedItems.Any(c => c.id == x.EntityId.ToString()))
                                             {
                                                 remove.Add(x);
                                             }
                                         });
            }
            remove.ForEachItem(trainer.RemoveClient);
            return trainer;
        }
    }

    public class TrainerViewModel:ViewModel
    {
        public string _deleteUrl { get; set; }
        public IEnumerable<SelectListItem> _StateList { get; set; }
        public IEnumerable<SelectListItem> _StatusList { get; set; }
        public TCRTokenInputViewModel ClientsDtos { get; set; }
        public TokenInputViewModel UserRolesDtos { get; set; }

        public bool DeleteImage { get; set; }
        public string Password { get; set; }
        [ValidateSameAs("Password")]
        public string PasswordConfirmation { get; set; }
        [ValidateNonEmpty]
        public string FirstName { get; set; }
        [ValidateNonEmpty]
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Color { get; set; }
        public string UserLoginInfoLoginName { get; set; }
        [ValidateNonEmpty]
        public string Email { get; set; }
        [ValidateNonEmpty]
        public string PhoneMobile { get; set; }
        public string SecondaryPhone { get; set; }
        public int ClientRateDefault { get; set; }
    }
    public class TCRTokenInputDto:TokenInputDto
    {
        public int percentage { get; set; }
        public string display { get { return name + "(" + percentage + ")"; } }

    }

    public class TCRTokenInputViewModel : ITokenInputViewModel
    {
        public IEnumerable<TCRTokenInputDto> _availableItems { get; set; }
        public IEnumerable<TCRTokenInputDto> selectedItems { get; set; }
    }
}   