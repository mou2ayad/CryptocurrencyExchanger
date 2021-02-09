using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace App.Components.Utilities.ErrorHandling
{
    public static class ExceptionMiddlewareExtention
    {
        public static void ConfigurErrorHandler(this IApplicationBuilder app, ILogger log, string AppName, bool IsDevelopment)
        {
            app.UseExceptionHandler(appError => {
                appError.Run(async context => {

                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Exception exception = contextFeature.Error;
                        if (contextFeature.Error is AggregateException)
                            exception = exception.InnerException;
                        HttpExceptionDetails errorDetails;
                        if (exception.IsClientException())
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            errorDetails = new HttpExceptionDetails()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                ErrorMessage = exception.Message
                            };
                        }
                        else
                        {

                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            errorDetails = new HttpExceptionDetails()
                            {
                                StatusCode = (int)HttpStatusCode.InternalServerError,
                                ErrorMessage = "Internal Server Error"
                            };

                        }

                        if (IsDevelopment)
                            errorDetails.ErrorMessage = exception.ToString();

                        if (exception.IsWithNoLog())
                            errorDetails.WithNolog();
                        else
                        {
                            var Parameters = JsonConvert.SerializeObject(context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()));
                            if (exception.IsLoggedAsInfo())
                            {
                                log.LogInformation(exception, "ErorrCode:{0} Service:{1} Path:{2} Parameters:{3}\n Exception=> ", errorDetails.ErrorCode, AppName, context.Request.Path.Value, Parameters);
                            }
                            else
                            {
                                log.LogError(exception, "ErorrCode:{0} Service:{1} Path:{2} Parameters:{3}\n Exception=> ", errorDetails.ErrorCode, AppName, context.Request.Path.Value, Parameters);

                            }

                        }
                        await context.Response.WriteAsync(errorDetails.ToString());
                    }
                });
            });

        }
    }
}
