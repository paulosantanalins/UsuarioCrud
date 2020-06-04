using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UsuarioApi.Api.Handlers
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

                var body = "";
                var req = context.Request;
                req.EnableRewind();
                req.Body.Position = 0;
                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    body = reader.ReadToEnd();
                }
                req.Body.Position = 0;

                string causa = "Default";
                if (exception.InnerException != null)
                {
                    causa = exception.InnerException.Message;
                }

                var path = context.Request.Path;
                var usuario = context.Request.Headers["Usuario"];
                Log.Fatal(causa + ":\n" + exception.ToString().Substring(0, 1000) + "\n" + path + "\nUsuario: " + usuario + "\n" + body + "\n");

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Mensagem = exception.Message,
                    Status = httpStatus
                }));
            }
        }
    }
}
