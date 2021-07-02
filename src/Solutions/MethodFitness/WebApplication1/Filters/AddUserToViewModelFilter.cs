using System.Linq;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
using MF.Core.Config;
using MF.Core.Domain;
using Microsoft.AspNetCore.Mvc.Filters;
using StructureMap;

namespace MF.Web.Filters
{
    public class AddUserToViewModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var repository = ObjectFactory.Container.GetInstance<IRepository>();
            var customPrincipal = (CustomPrincipal) filterContext.HttpContext.User;
            var actionParam = filterContext.ActionParameters.FirstOrDefault(x => x.Value is ViewModel || x.Value.GetType().IsSubclassOf(typeof (ViewModel)));
            if (actionParam.Value!=null)
            {
                var user = repository.Find<User>(customPrincipal.UserId);
                ((ViewModel)actionParam.Value).User = user;
            }        
        }
    }
}