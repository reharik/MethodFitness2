using CC.Security;

namespace CC.Core.Services
{
    public interface ICCSessionContext
    {
        IUser GetCurrentUser();
        int GetUserId();
        string MapPath(string url);
    }
}