using Cadastro.Infra.CrossCutting.IoC;
using Logger.Model;
using Logger.Repository.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Api.Handler
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
                string mensagemException = "";
                if (exception.InnerException != null)
                {
                    causa = exception.InnerException.Message;
                }
                else if (exception.Message != null)
                {
                    mensagemException = exception.Message;
                }

                var variables = RecuperarVariablesToken();

                var path = context.Request.Path;
                var usuario = variables.UserName;

                causa = causa + " | URL: " + path + " | Usuario: " + usuario + " | Body: " + body;
                var _logGenericoRepository = RecuperarLogGenericoRepository();
                await _logGenericoRepository.AddLog(new LogGenerico
                {
                    NmOrigem = "HANDLER",
                    DescExcecao = exception.ToString(),
                    DescLogGenerico = causa,
                    DtHoraLogGenerico = DateTime.Now,
                    NmTipoLog = "ERRO"
                });

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Mensagem = causa,
                    Status = httpStatus,
                    MensagemException = mensagemException
                }));
            }
        }

        private static ILogGenericoRepository RecuperarLogGenericoRepository()
        {
            var logGenericoRepository = Injector.ServiceProvider.GetService(typeof(ILogGenericoRepository)) as ILogGenericoRepository;
            return logGenericoRepository;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
