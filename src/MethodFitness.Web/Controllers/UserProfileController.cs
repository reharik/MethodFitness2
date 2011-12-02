using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using System.Linq;

namespace MethodFitness.Web.Controllers
{
    public class UserProfileController:MFController
    {
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;
        private readonly ISecurityDataService _securityDataService;
        private readonly ISaveEntityService _saveEntityService;
        private readonly IUpdateCollectionService _updateCollectionService;
        private readonly ISelectListItemService _selectListItemService;

        public UserProfileController(IRepository repository,
                                     ISessionContext sessionContext,
                                     ISecurityDataService securityDataService,
                                     ISaveEntityService saveEntityService,
            IUpdateCollectionService updateCollectionService,
            ISelectListItemService selectListItemService)
        {
            _repository = repository;
            _sessionContext = sessionContext;
            _securityDataService = securityDataService;
            _saveEntityService = saveEntityService;
            _updateCollectionService = updateCollectionService;
            _selectListItemService = selectListItemService;
        }

        public ActionResult UserProfile(ViewModel input)
        {
            var userId = _sessionContext.GetUserEntityId();
            var user = _repository.Find<User>(userId);
            var model = new UserViewModel {User = user,
                Title = WebLocalizationKeys.MY_ACCOUNT_INFORMATION.ToString()
            };
            return View(model);
        }

        public JsonResult Save(UserViewModel input)
        {
           
            var origional = _repository.Find<User>(input.EntityId);
            origional.BirthDate = input.User.BirthDate;
            origional.FirstName = input.User.FirstName;
            origional.MiddleInitial = input.User.MiddleInitial;
            origional.LastName = input.User.LastName;
            origional.BirthDate= input.User.BirthDate;

            handlePassword(input, origional);
            mapCollections(origional, input);
           
            var crudManager = _saveEntityService.ProcessSave(origional);
            var notification = crudManager.Finish();
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        private void mapCollections(User origional, UserViewModel input)
        {
           
        }

        private void handlePassword(UserViewModel input, User origional)
        {
            if (input.Password.IsNotEmpty())
            {
                var loginInfo = origional.UserLoginInfo;
                loginInfo.Salt = _securityDataService.CreateSalt();
                loginInfo.Password = _securityDataService.CreatePasswordHash(input.Password,
                                                            loginInfo.Salt);
            }
        }

        public ActionResult EmailConformation(ViewModel input)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UserViewModel : ViewModel
    {
        public UserViewModel()
        {
        }

        public User User { get; set; }
        public string Password { get; set; }
        [ValidateSameAs("Password")]
        public string RepeatPassword { get; set; }
        public IEnumerable<TokenInputDto> AvailableItems { get; set; }
        public IEnumerable<TokenInputDto> SelectedItems { get; set; }

        public bool DeleteImage { get; set; }
    }

    

}