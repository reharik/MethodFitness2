﻿using System.Collections.Generic;
using CC.Core.Core.ValidationServices;
using CC.Core.DataValidation;
using Microsoft.AspNetCore.Mvc;

namespace CC.Core.Core.DomainTools
{
    /// <summary>
    /// Communicates (notifies) the result of an action from the server to the client
    /// </summary>
    public class Notification : JsonResult
    {
        public Notification(bool Success, string Message) : base( new{ Success=Success, Message=Message})
        {
        }

        public Notification(Notification report) : base(report)
        {
            Message = report.Message;
            Success = report.Success;
            Errors = report.Errors;
            //Target = continuation.Target;
        }

        public Notification(Continuation report) : base(report)
        {
            Errors = new List<ErrorInfo>();
            Message = report.Message;
            Success = report.Success;
            Errors = report.Errors;
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