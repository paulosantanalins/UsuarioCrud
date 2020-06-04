using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LancamentoFinanceiro.Api.Handler
{
    public class CustomExceptionHandler
    {
        public async Task Invoke(HttpContext context)
        {
            var httpStatus = HttpStatusCode.InternalServerError;

            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception != null)
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)httpStatus;

                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    string text = reader.ReadToEnd();
                }

                string causa = "Default";
                if (exception.InnerException != null)
                {
                    causa = exception.InnerException.Message;
                }
                Console.WriteLine(causa);

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Mensagem = exception.Message,
                    Status = httpStatus
                }));
            }
        }
    }
}
