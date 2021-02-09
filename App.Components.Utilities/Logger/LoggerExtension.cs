using System;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace App.Components.Utilities.Logger
{
    public static class LoggerExtension
    {
        public static void LogErrorDetails(this ILogger logger, Exception ex, params object[] parameters)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            var ParametersJson = parameters == null ? "" : JsonConvert.SerializeObject(parameters.Where(e => e != null).ToArray());
            logger.LogError(ex, "ErorrCode:{0} Mathod:{1}.{2} Parameters:{3}\n Exception=>",
                Guid.NewGuid(), method.Name, method.ReflectedType.Name, ParametersJson);
        }        
    }
}
