using System;

namespace App.Components.Utilities.ErrorHandling
{
    public static class ClientExceptionExtension
    {
      
        public static Exception MarkAsClientException(this Exception exception)
        {
            if(!exception.Data.Contains("ClientException"))
                exception.Data.Add("ClientException", true);
            return exception;
        }
        public static Exception MarkAsNotClientException(this Exception exception)
        {
            if (exception.Data.Contains("ClientException"))
                exception.Data.Remove("ClientException");
            return exception;
        }
        public static bool IsClientException(this Exception exception) => exception.Data.Contains("ClientException");
        
        public static Exception WithNoLog(this Exception exception)
        {
            if (!exception.Data.Contains("WithNoLog"))
                exception.Data.Add("WithNoLog", true);
            return exception;
        }
        public static Exception WithLog(this Exception exception)
        {
            if (exception.Data.Contains("WithNoLog"))
                exception.Data.Remove("WithNoLog");

            return exception;
        }
        public static bool IsWithNoLog(this Exception exception) => exception.Data.Contains("WithNoLog");
              
        
        public static Exception LogAsInfo(this Exception exception)
        {
            if (!exception.Data.Contains("AsInfo"))
                exception.Data.Add("AsInfo", true);
            return exception;
        }
        public static bool IsLoggedAsInfo(this Exception exception) => exception.Data.Contains("AsInfo");
             
    }
}
