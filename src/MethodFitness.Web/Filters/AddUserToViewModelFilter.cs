using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using MethodFitness.Core.Config;
using MethodFitness.Core.Domain;
using StructureMap;

namespace MethodFitness.Web.Filters
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