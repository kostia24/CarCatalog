using System;
using System.Diagnostics;

namespace CarsCatalog.Models
{
    [DebuggerDisplay("Status: {Status}")]
    public class OperationStatus
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string ExeptionMessage { get; set; }
        public string ExeptionStackTrace { get; set; }

        public static OperationStatus CreateFromExeption(string message, Exception ex)
        {
            OperationStatus status = new OperationStatus(){Status = false, Message = message};
            if (ex != null)
            {
                status.ExeptionMessage = ex.Message;
                status.ExeptionStackTrace = ex.StackTrace;
            }
            return status;
        }
    }
}