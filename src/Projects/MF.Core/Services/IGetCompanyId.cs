using MF.Core.Config;
using Microsoft.AspNetCore.Http;

namespace MF.Core.Services
{

    // this class is here so we can have a repository in the ISessionContext and still 
    // get the companyId from IUnitOfWork
    public interface IGetCompanyIdService
    {
        int Execute();
    }


    public class GetCompanyIdService : IGetCompanyIdService
    {
        public int Execute()
        {
            var httpContext = HttpContextHelper.Current;
            var customPrincipal = httpContext != null ? httpContext.User as CustomPrincipal : null;
            return customPrincipal != null ? customPrincipal.CompanyId : 1;
        }
    }

    public class DataLoaderGetCompanyIdService : IGetCompanyIdService
    {
        public int Execute()
        {
            return 1;
        }
    }
}