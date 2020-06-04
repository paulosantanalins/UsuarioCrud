using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Service.Interfaces;
using System.Net;
using System.Net.Mail;
using System;
using EnvioEmail.Domain.EmailRoot.Repository;
using System.Collections.Generic;
using System.Linq;
using EnvioEmail.Domain.SharedRoot;
using Utils;

namespace EnvioEmail.Domain.EmailRoot.Service
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailRepository _emailRepository;
        private readonly ITemplateEmailRepository _templateEmailRepository;
        private readonly IParametroTemplateRepository _parametroTemplateRepository;
        private readonly IVariablesToken _variables;

        public EmailService(
            IEmailRepository emailRepository,
            ITemplateEmailRepository templateEmailRepository,
            IParametroTemplateRepository parametroTemplateRepository,            
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _unitOfWork = unitOfWork;
            _variables = variables;
            _emailRepository = emailRepository;
            _templateEmailRepository = templateEmailRepository;
            _parametroTemplateRepository = parametroTemplateRepository;
        }

        public IEnumerable<Email> BuscarEmailsPendentes()
        {
            var emails = _emailRepository.BuscarEmailsPendentes();
            return emails;
        }

        public Email Enviar(Email email)
        {           
            try
            {
                SmtpClient smtpClient = DefinirCredenciais();
                MailMessage mensagem = MontarMensagem(email);

                email.Assunto = mensagem.Subject;
                email.Corpo = mensagem.Body;

                if (!email.DtParaEnvio.HasValue || (email.DtParaEnvio.HasValue && email.DtParaEnvio <= DateTime.Now))
                {         
                    smtpClient.Send(mensagem);
                    email.DtCadastro = DateTime.Now;
                    email.DtEnvio = DateTime.Now;
                    email.Status = "S";
                }
                else
                {
                    email.Status = "A";
                }
                Adicionar(email);

                return email;
            }
            catch (Exception ex)
            {
                AdicionarFalhaAoEnviarEmailNovo(email, ex);
                
            }
            return null;
        }

        public void Reenviar(Email email)
        {
            try
            {
                SmtpClient smtpClient = DefinirCredenciais();
                MailMessage mensagem = MontarMensagemReenvio(email);

                if (!email.DtParaEnvio.HasValue || (email.DtParaEnvio.HasValue && email.DtParaEnvio <= DateTime.Now))
                {
                    if (email.TentativasComErro > 5)
                    {
                        email.Status = "C";
                    }
                    else
                    {
                        smtpClient.Send(mensagem);
                        email.DtEnvio = DateTime.Now;
                        email.Status = "S";
                    }
                    Atualizar(email);
                }
            }
            catch (Exception ex)
            {
                AdicionarFalhaAoEnviarEmailEmNovaTentativa(email, ex);
            }
        }

        private MailMessage MontarMensagem(Email email)
        {
            MailMessage mensagem = new MailMessage();
            mensagem.IsBodyHtml = true;
            mensagem.From = new MailAddress(_variables.EmailRemetente, email.RemetenteNome);
            if (!String.IsNullOrEmpty(email.RemetenteEmail))
            {
                mensagem.From = new MailAddress(email.RemetenteEmail, email.RemetenteNome);
            }

            PreencherDestinatarios(email, mensagem);

            if (!email.IdTemplate.HasValue)
            {
                mensagem.Subject = email.Assunto;
                mensagem.Body = email.Corpo;
            }
            else
            {
                var template = _templateEmailRepository.BuscarPorIdComParametros(email.IdTemplate.Value);
                if (template != null)
                {
                    mensagem.Subject = template.Assunto;
                    if (!String.IsNullOrEmpty(email.Assunto))
                    {
                        mensagem.Subject = email.Assunto;
                    }
                    mensagem.Body = PreencherCorpoViaTemplate(template, email);
                }
                else
                {
                    throw new Exception("Template inexistente.");
                }
            }
            return mensagem;
        }

        private MailMessage MontarMensagemReenvio(Email email)
        {
            MailMessage mensagem = new MailMessage();
            mensagem.IsBodyHtml = true;

            mensagem.From = new MailAddress(_variables.EmailRemetente, email.RemetenteNome);
            if (!String.IsNullOrEmpty(email.RemetenteEmail))
            {
                mensagem.From = new MailAddress(email.RemetenteEmail, email.RemetenteNome);
            }
            PreencherDestinatarios(email, mensagem);

            mensagem.Body = email.Corpo;
            mensagem.Subject = email.Assunto;

            return mensagem;
        }

        private static string PreencherCorpoViaTemplate(TemplateEmail template, Email email)
        {
            var corpo = template.Corpo;

            foreach (var valorParametro in email.ValoresParametro)
            {
                corpo = corpo.Replace(valorParametro.ParametroNome, valorParametro.ParametroValor);
            }

            return corpo;
        }

        private static void PreencherDestinatarios(Email email, MailMessage mensagem)
        {
            if (!String.IsNullOrEmpty(email.Para))
            {
                foreach (var destinatario in email.Para.Split(','))
                {
                    mensagem.To.Add(new MailAddress(destinatario));
                }
            }
            if (!String.IsNullOrEmpty(email.ComCopia))
            {
                foreach (var destinatario in email.ComCopia.Split(','))
                {
                    mensagem.CC.Add(new MailAddress(destinatario));
                }
            }
            if (!String.IsNullOrEmpty(email.ComCopiaOculta))
            {
                foreach (var destinatario in email.ComCopiaOculta.Split(','))
                {
                    mensagem.Bcc.Add(new MailAddress(destinatario));
                }
            }
        }

        private static SmtpClient DefinirCredenciais()
        {
            var smtpClient = new SmtpClient();
            smtpClient.Host = Variables.Host;
            smtpClient.Port = Variables.Port;
            smtpClient.UseDefaultCredentials = Variables.UseDefaultCredentials;
            smtpClient.EnableSsl = Variables.EnableSsl;
            return smtpClient;
        }

        private void Adicionar(Email email)
        {
            email.ValoresParametro = null;
            _emailRepository.Adicionar(email);
            _unitOfWork.Commit();
        }

        private void Atualizar(Email email)
        {
            try
            {
                email.ValoresParametro = null;
                _emailRepository.Update(email);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void AdicionarFalhaAoEnviarEmailNovo(Email email, Exception ex)
        {
            email.TentativasComErro = 1;
            email.Status = "E";
            email.Erro = ex.Message;
            email.DtCadastro = DateTime.Now;
            Adicionar(email);
        }

        private void AdicionarFalhaAoEnviarEmailEmNovaTentativa(Email email, Exception ex)
        {
            email.TentativasComErro = email.TentativasComErro.HasValue ? email.TentativasComErro.Value + 1 : 1;
            email.Status = "E";
            email.Erro = ex.Message;
            Atualizar(email);
        }

    }
}
