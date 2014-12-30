using System.Collections.Generic;
using CC.Core.DataValidation;

namespace CC.Core.Core.ValidationServices
{
    public class Continuation
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<ErrorInfo> Errors { get; set; }
        public object Payload { get; set; }
    }
}