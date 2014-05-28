using System.Collections.Generic;
using System.Web.Mvc;
using xVal.ServerSide;

namespace CC.Core.DomainTools
{
    /// <summary>
    /// Communicates (notifies) the result of an action from the server to the client
    /// </summary>
    public class Notification : JsonResult
    {
        public Notification()
        {
        }

        public Notification(Notification report)
        {
            Message = report.Message;
            Success = report.Success;
            Errors = report.Errors;
            //Target = continuation.Target;
        }

        public List<ErrorInfo> Errors;
        public string Message { get; set; }
        public bool Success { get; set; }
        public string RedirectUrl { get; set; }
        public bool Redirect { get; set; }
        public long EntityId { get; set; }
        public string Variable { get; set; }
        public object Payload { get; set; }

    }
}