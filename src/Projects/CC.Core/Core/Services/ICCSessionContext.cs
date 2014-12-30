using CC.Core.Security;

namespace CC.Core.Core.Services
{
    public interface ICCSessionContext
    {
        IUser GetCurrentUser();
        int GetUserId();
        string MapPath(string url);
    }
}