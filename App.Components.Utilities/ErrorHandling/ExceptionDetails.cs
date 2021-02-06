using Newtonsoft.Json;
using System;

namespace App.Components.Utilities.ErrorHandling
{
    public class ExceptionDetails
    {
        public string ErrorCode { protected set; get; }
        public string ErrorMessage { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        public ExceptionDetails()
        {
            ErrorCode = Guid.NewGuid().ToString();
        }
    }
    public class HttpExceptionDetails : ExceptionDetails
    {
        public int StatusCode { get; set; }
        public HttpExceptionDetails() : base()
        {            
        }
        public HttpExceptionDetails WithNolog()
        {
            ErrorCode = null;
            return this;
        }
    }
}
