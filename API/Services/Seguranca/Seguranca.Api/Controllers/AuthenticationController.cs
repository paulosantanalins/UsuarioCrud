using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using Seguranca.Api.Configurations;
using Seguranca.Api.ViewModels;
using Seguranca.Domain.Core.Notifications;
using Seguranca.Domain.UsuarioRoot;
using Seguranca.Infra.CrossCutting.IoC;
using Seguranca.Infra.CrossCutting.IoC.JWT;
using Seguranca.Infra.CrossCutting.IoC.JwtUtils;
using Seguranca.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Base;


namespace Seguranca.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Authentication")]
    [ApiVersion("1", Deprecated = true)]
    [ApiVersion("2")]
    public class AuthenticationController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly UserManager<Usuario> _userManager;
        private readonly IdentityContext _context;
        private readonly IConfiguration _configuration;
        private readonly LdapSeguranca[] _ldapConfig;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly List<AplicacaoVM> _aplicacoes;

        public AuthenticationController(NotificationHandler notificationHandler,
                                 IJwtFactory jwtFactory,
                                 IOptions<JwtIssuerOptions> jwtOptions,
                                 UserManager<Usuario> userManager,
                                 IdentityContext context,
                                 IConfiguration configuration,
                                 LdapSeguranca[] ldapConfig,
                                 MicroServicosUrls microServicosUrls,
                                 List<AplicacaoVM> aplicacoesVM)
        {
            _notificationHandler = notificationHandler;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _ldapConfig = ldapConfig;
            _aplicacoes = aplicacoesVM;
            _microServicosUrls = microServicosUrls;
        }

        [AllowAnonymous]
        [HttpGet("login-transporte")]
        [MapToApiVersion("2")]
        public IActionResult PostTranposte()
        {
            return Ok(Variables.EnvironmentName);

        }

        /// <summary>
        /// Realiza o login através do Transporte via Windows Authentication + JWT
        /// </summary>
        /// <response code="200">Retorna um objeto com login, nome completo do usuário e um auth_token</response>
        [AllowAnonymous]
        [HttpPost("login-transporte")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> PostTranposte([FromBody] TransporteVM credentials)
        {
            string nomeCompleto = "", email = "", cel = "", uidNumber = "", celTratada = "";

            try
            {
                var decrypted = Decrypt.DecryptString(credentials.Chave, _configuration["Transporte:Chave"]);
                if (!decrypted.Contains(credentials.Usuario.ToUpper().Substring(0, 3)))
                {
                    return StatusCode(401, new { Name = "Token Invalido." });
                }
                decrypted = decrypted.Replace(credentials.Usuario.ToUpper().Substring(0, 3), "");
                DateTime dataTransporte = DateTime.ParseExact(decrypted, "ddMMyyyyHH", null);
                if (dataTransporte < DateTime.UtcNow.AddHours(-1))
                {
                    return StatusCode(401, new { Name = "Token Expirou." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(401, new { Name = "Sem Autorização." });
            }

            using (var cn = new LdapConnection())
            {
                try
                {
                    cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);

                    cn.Bind(String.Format("stefanini-dom\\{0}", _ldapConfig.FirstOrDefault().LoginAD), _ldapConfig.FirstOrDefault().Password);

                    var searchBase = string.Empty;
                    string filterInformacaoUsuario = "(sAMAccountName=" + credentials.Usuario + ")";
                    var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);

                    if (search.hasMore())
                    {
                        var nextEntry = search.next();
                        var user = nextEntry.getAttributeSet();
                        nomeCompleto = (user.getAttribute("displayName") != null) ? user.getAttribute("displayName").StringValue : "";
                        email = (user.getAttribute("mail") != null) ? user.getAttribute("mail").StringValue : "";
                        cel = (user.getAttribute("department") != null) ? user.getAttribute("department").StringValue.Split('|')[0] : "";
                        celTratada = (user.getAttribute("department") != null) ? TratarCelula(user.getAttribute("department").StringValue.Split('|')[0]) : "";
                        uidNumber = (user.getAttribute("uidNumber") != null) ? user.getAttribute("uidNumber").StringValue : "";
                    }
                }
                catch (LdapException e)
                {
                    return StatusCode(401, new { Name = "Sem Autorização." });
                }
            }

            try
            {
                var celulasVisualizadas = ObterCelulasVisualizadas(credentials.Usuario, Convert.ToInt32(celTratada));

                var identity = _jwtFactory.GenerateClaimsIdentity(credentials.Usuario, nomeCompleto, email, null, cel, uidNumber, celulasVisualizadas, celTratada);
                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Usuario, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
                var tokenLogin = await GerarTokenLoginUsuario(credentials.Usuario);
                var tokens = new List<string>();
                tokens.Add(jwt);
                tokens.Add(tokenLogin);
                Console.WriteLine("Login realizado: " + nomeCompleto);
                return new OkObjectResult(tokens);
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Realiza o login através do AD
        /// </summary>
        /// <response code="200">Retorna um objeto com login, nome completo do usuário e um auth_token</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Post([FromBody] LoginVM credentials)
        {
            string nomeCompleto = "", email = "", cel = "", uidNumber = "", celTratada = "";

            var aplicacaoValida = _aplicacoes.Any(x => x.Nome.Equals(credentials.Aplicacao) && x.Senha.Equals(credentials.AplicacaoSenha));
            if (!aplicacaoValida)
            {
                throw new Exception("Aplicação não reconhecida pelo sistema.");
            }

            using (var cn = new LdapConnection())
            {
                try
                {
                    cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                    cn.Bind(String.Format("stefanini-dom\\{0}", credentials.UserName), credentials.Password);
                    var searchBase = string.Empty;
                    string filterInformacaoUsuario = "(sAMAccountName=" + credentials.UserName + ")";
                    var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);

                    if (search.hasMore())
                    {
                        var nextEntry = search.next();
                        var user = nextEntry.getAttributeSet();
                        nomeCompleto = (user.getAttribute("displayName") != null) ? user.getAttribute("displayName").StringValue : "";
                        email = (user.getAttribute("mail") != null) ? user.getAttribute("mail").StringValue : "";
                        cel = (user.getAttribute("department") != null) ? user.getAttribute("department").StringValue.Split('|')[0] : "";
                        celTratada = (user.getAttribute("department") != null) ? TratarCelula(user.getAttribute("department").StringValue.Split('|')[0]) : "";
                        uidNumber = (user.getAttribute("uidNumber") != null) ? user.getAttribute("uidNumber").StringValue : "";
                    }
                }
                catch (LdapException e)
                {
                    return null;
                }
            }


            var celulasVisualizadas = ObterCelulasVisualizadas(credentials.UserName, Convert.ToInt32(celTratada));

            var identity = _jwtFactory.GenerateClaimsIdentity(credentials.UserName, nomeCompleto, email, credentials.Password, cel, uidNumber, celulasVisualizadas, celTratada);
            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            var tokenLogin = await GerarTokenLoginUsuario(credentials.UserName);
            var tokens = new List<string>();
            tokens.Add(jwt);
            tokens.Add(tokenLogin);
            Console.WriteLine("Login realizado: " + nomeCompleto);
            return new OkObjectResult(tokens);
        }

        /// <summary>
        /// Realiza o login através do AD
        /// </summary>
        /// <response code="200">Retorna um objeto com login, nome completo do usuário e um auth_token</response>
        [AllowAnonymous]
        [HttpPost("loginEmail")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> LoginEmail([FromBody] LoginEmailVM credentials)
        {
            int indexBuscaEmail = 0;
            int indexLdap = 0;
            string userName = "", nomeCompleto = "", cel = "", uidNumber = "", celTratada = "";
            string emailStefanini = string.Empty;
            string dados = string.Empty;

            var aplicacaoValida = _aplicacoes.Any(x => x.Nome.Equals(credentials.Aplicacao) && x.Senha.Equals(credentials.AplicacaoSenha));
            if (!aplicacaoValida)
            {
                throw new Exception("Aplicação não reconhecida pelo sistema.");
            }

            try
            {
                if (!credentials.Email.Contains("@stefanini"))
                {
                    emailStefanini = this.BuscarEmailStefanini(indexBuscaEmail, credentials.Email);

                    while (indexBuscaEmail <= _ldapConfig.Length - 1)
                    {
                        if (emailStefanini != "")
                        {
                            credentials.Email = emailStefanini;
                            indexLdap = indexBuscaEmail;
                            break;
                        }
                        else
                        {
                            indexBuscaEmail++;
                            emailStefanini = this.BuscarEmailStefanini(indexBuscaEmail, credentials.Email);
                        }
                    }
                }

                dados = this.ConectarDAP(indexLdap, credentials.Email, credentials.Password);

                while (indexLdap <= _ldapConfig.Length - 1)
                {
                    if (dados != "")
                    {
                        string[] arrayDados = dados.Split(",");

                        userName = arrayDados[0];
                        nomeCompleto = arrayDados[1];
                        cel = arrayDados[2];
                        celTratada = arrayDados[3];
                        uidNumber = arrayDados[4];

                        break;
                    }
                    else
                    {
                        indexLdap++;
                        dados = this.ConectarDAP(indexLdap, credentials.Email, credentials.Password);
                    }
                }
            }
            catch (LdapException e)
            {
                return null;
            }

            if (dados != "")
            {
                var celulasVisualizadas = ObterCelulasVisualizadas(userName);
                var identity = _jwtFactory.GenerateClaimsIdentity(userName, nomeCompleto, credentials.Email, credentials.Password, cel, uidNumber, celulasVisualizadas, celTratada);
                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, userName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
                var tokenLogin = await GerarTokenLoginUsuario(userName);
                var tokens = new List<string>();
                tokens.Add(jwt);
                tokens.Add(tokenLogin);
                Console.WriteLine("Login realizado: " + nomeCompleto);
                return new OkObjectResult(tokens);
            }
            else

            {
                throw new LdapException("user not found", 525, "user not found");
            }
        }

        [Authorize("Bearer")]
        [HttpGet("simular-login/{userName}")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> SimularLogin(string userName)
        {
            try
            {
                string nomeCompleto = "", email = "", login = "";

                using (var cn = new LdapConnection())
                {
                    try
                    {
                        cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                        cn.Bind(String.Format("stefanini-dom\\{0}", "almintegration"), "stefanini@10");
                        var searchBase = string.Empty;
                        string filterInformacaoUsuario = "(sAMAccountName=" + userName + ")";
                        var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);

                        if (search.hasMore())
                        {
                            var nextEntry = search.next();
                            var user = nextEntry.getAttributeSet();
                            nomeCompleto = user.getAttribute("cn").StringValue;
                            email = user.getAttribute("mail").StringValue;
                            login = user.getAttribute("sAMAccountName").StringValue;
                        }
                    }
                    catch (LdapException e)
                    {
                        return null;
                    }
                }

                var identity = _jwtFactory.GenerateClaimsIdentity(login, nomeCompleto, email);
                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, login, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
                var tokenLogin = await GerarTokenLoginUsuario(login);
                var tokens = new List<string>();
                tokens.Add(jwt);
                tokens.Add(tokenLogin);
                var informacoesUsuario = ObterPerfisUsuario(login);
                return Ok(new { tokens, perfil = informacoesUsuario.nmPerfil, informacoesUsuario.funcionalidades, email, nomeCompleto });

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Authorize("Bearer")]
        [HttpPost("VerificarDominio")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> PostVerificarDominio([FromBody]TokenViewModel tokenVM)
        {
            var nomeCompleto = "";
            var email = "";
            var login = "";
            var ipCliente = Request.HttpContext.Connection.RemoteIpAddress;
            var nomeMaquina = Dns.GetHostEntry(ipCliente);
            var dominio = nomeMaquina.HostName.Split(".").ToList();
            var dominioValido = dominio.Contains("stefanini");
            var tokenAtual = await GerarTokenLoginUsuario(tokenVM.Login);

            if (dominioValido && tokenAtual == tokenVM.Token)
            {
                using (var cn = new LdapConnection())
                {
                    try
                    {
                        cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                        cn.Bind(String.Format("stefanini-dom\\{0}", "almintegration"), "stefanini@10");
                        var searchBase = string.Empty;
                        string filterInformacaoUsuario = "(sAMAccountName=" + tokenVM.Login + ")";
                        var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);

                        if (search.hasMore())
                        {
                            var nextEntry = search.next();
                            var user = nextEntry.getAttributeSet();
                            nomeCompleto = user.getAttribute("cn").StringValue;
                            email = user.getAttribute("mail").StringValue;
                            login = user.getAttribute("sAMAccountName").StringValue;
                        }
                    }
                    catch (LdapException e)
                    {
                        return null;
                    }
                }
                var identity = _jwtFactory.GenerateClaimsIdentity(login, nomeCompleto, email);

                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, login, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
                return new OkObjectResult(jwt);
            }
            return BadRequest();
        }

        [Authorize("Bearer")]
        [HttpPost("VerificarToken/")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> PostVerificarToken([FromBody]TokenViewModel token)
        {
            var tokenValido = new JwtSecurityTokenHandler().CanReadToken(token.Token);
            if (tokenValido)
            {
                var conteudo = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);
                var claims = conteudo.Claims.Where(x => x.Type == "nomeCompleto" || x.Type == "login" || x.Type == "email" || x.Type == "celula" || x.Type == "uidNumber");
                var informacoesFormatada = new InformacoesUsuarioViewModel
                {
                    Email = claims.FirstOrDefault(x => x.Type == "email").Value,
                    NomeCompleto = claims.FirstOrDefault(x => x.Type == "nomeCompleto").Value,
                    Login = claims.FirstOrDefault(x => x.Type == "login").Value,
                    Celula = claims.FirstOrDefault(x => x.Type == "celula").Value != null ? claims.FirstOrDefault(x => x.Type == "celula").Value.Split(' ')[1] : "",
                    UidNumber = claims.FirstOrDefault(x => x.Type == "uidNumber").Value
                };
                return await Task.Run(() => Ok(new { dados = informacoesFormatada, notifications = "", success = true }));
            }
            else
            {
                return await Task.Run(() => Ok(new { dados = "", notifications = "Token inválido", success = false }));
            }
        }

        [HttpPost("BuscarUsuariosAdPorCelula")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> BuscarUsuariosAdPorCelula([FromBody] FiltroAdVM filtro)
        {
            InformacoesUsuarioViewModel informacoesFormatada = null;
            List<UsuarioAdVM> usuarios = new List<UsuarioAdVM>();
            List<string> filters = new List<string>();
            //var tokenValido = new JwtSecurityTokenHandler().CanReadToken(filtro.Token);
            //if (tokenValido)
            //{
            //var conteudo = new JwtSecurityTokenHandler().ReadJwtToken(filtro.Token);
            //var claims = conteudo.Claims.Where(x => x.Type == "nomeCompleto" || x.Type == "login" || x.Type == "email");
            //informacoesFormatada = new InformacoesUsuarioViewModel
            //{
            //    Email = claims.FirstOrDefault(x => x.Type == "email").Value,
            //    NomeCompleto = claims.FirstOrDefault(x => x.Type == "nomeCompleto").Value,
            //    Login = claims.FirstOrDefault(x => x.Type == "login").Value
            //};
            foreach (var item in filtro.Celulas)
            {
                var unicaCelula = item;
                var celulaFiltrar = "";
                if (unicaCelula.Count() < 3)
                {
                    for (int i = 0; i < 3 - unicaCelula.Count(); i++)
                    {
                        celulaFiltrar += "0";
                    }
                }
                celulaFiltrar += unicaCelula;
                filters.Add("(&(department=CEL*" + celulaFiltrar + "*))||" +
                    "((userAccountControl=512)(userAccountControl=544)(userAccountControl=66048)(userAccountControl=66080)" +
                    "(userAccountControl=262656)(userAccountControl=262688)(userAccountControl=328192)(userAccountControl=328224))");
            }
            using (var cn = new LdapConnection())
            {
                try
                {
                    cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                    cn.Bind(String.Format("stefanini-dom\\{0}", "almintegration"), "stefanini@10");
                    var searchBase = string.Empty;
                    foreach (var item in filters)
                    {
                        var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, item, new string[] { "sAMAccountName", "department", "cn" }, false);
                        while (search.hasMore())
                        {
                            try
                            {
                                var nextEntry = search.next();
                                var user = nextEntry.getAttributeSet();
                                usuarios.Add(new UsuarioAdVM
                                {
                                    Login = user.getAttribute("sAMAccountName").StringValue,
                                    NomeCompleto = user.getAttribute("cn").StringValue,
                                    Celula = user.getAttribute("department").StringValue.Split('|')[0]
                                });

                            }
                            catch (Exception e)
                            {
                                break;
                            }
                        }
                    }
                    return await Task.Run(() => Ok(new
                    {
                        dados = usuarios.OrderBy(x => x.Login).ToList(),
                        notifications = "",
                        success = true
                    }));
                }
                catch (LdapException e)
                {
                    return BadRequest();
                }
                //}
            }
            return BadRequest();

        }

        /// <summary>
        /// Realiza uma pesquisa no AD pelo login
        /// </summary>
        /// <response code="200">Retorna uma lista de usuarios</response>
        [Authorize("Bearer")]
        [HttpGet("FiltrarUsuariosADPorLogin/{login}")]
        [ProducesResponseType(200)]
        [MapToApiVersion("2")]
        public async Task<IActionResult> FiltrarUsuariosADPorLogin(string login)
        {
            var usuariosAD = new List<UsuarioAdVM>();
            using (var cn = new LdapConnection())
            {
                cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                cn.Bind(String.Format("stefanini-dom\\{0}", "almintegration"), "stefanini@10");
                var searchBase = string.Empty;
                string filterInformacaoUsuario = "(sAMAccountName=" + login + ")";
                var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);
                while (search.hasMore())
                {
                    try
                    {
                        var nextEntry = search.next();
                        var user = nextEntry.getAttributeSet();
                        var usuario = new UsuarioAdVM();

                        usuario.Login = user.getAttribute("sAMAccountName").StringValue;
                        usuario.Celula = user.getAttribute("department").StringValue.Split('|')[0].Trim();
                        usuario.CPF = user.getAttribute("employeenumber").StringValue;
                        usuario.NomeCompleto = user.getAttribute("displayName").StringValue;
                        usuario.Cargo = user.getAttribute("title").StringValue;
                        usuario.Email = user.getAttribute("mail").StringValue;
                        usuario.DataNascimento = user.getAttribute("employeeId").StringValue;
                        usuario.IdEacesso = (user.getAttribute("uidNumber") != null) ? user.getAttribute("uidNumber").StringValue : "";

                        usuariosAD.Add(usuario);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return Ok(usuariosAD);
            }
        }

        /// <summary>
        /// Realiza uma pesquisa no AD pelo nome
        /// </summary>
        /// <response code="200">Retorna uma lista de usuarios</response>
        [Authorize("Bearer")]
        [HttpGet("filtrar-usuarios-por-nome/{nome}")]
        [ProducesResponseType(200)]
        [MapToApiVersion("2")]
        public async Task<IActionResult> FiltrarUsuariosADPorNome(string nome)
        {
            var usuariosAD = new List<UsuarioAdVM>();
            using (var cn = new LdapConnection())
            {
                cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                cn.Bind(String.Format("stefanini-dom\\{0}", "almintegration"), "stefanini@10");
                var searchBase = string.Empty;
                string filterInformacaoUsuario = "(displayName=*" + nome + "*)";
                var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);
                while (search.hasMore())
                {
                    try
                    {
                        var nextEntry = search.next();
                        var user = nextEntry.getAttributeSet();
                        var usuario = new UsuarioAdVM();

                        usuario.Login = user.getAttribute("sAMAccountName").StringValue;
                        usuario.Celula = user.getAttribute("department").StringValue.Split('|')[0].Trim();
                        usuario.CPF = user.getAttribute("employeenumber").StringValue;
                        usuario.NomeCompleto = user.getAttribute("displayName").StringValue;

                        usuariosAD.Add(usuario);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return Ok(usuariosAD);
            }
        }

        /// <summary>
        /// Realiza uma pesquisa no AD trazendo o e-mail dos usuários
        /// </summary>
        /// <response code="200">Retorna uma lista de emails</response>
        [HttpPost("obter-emails")]
        [ProducesResponseType(200)]
        [MapToApiVersion("2")]
        public async Task<IActionResult> ObterEmails([FromBody]List<string> logins)
        {
            List<string> emails = new List<string>();

            using (var cn = new LdapConnection())
            {
                cn.Connect(_ldapConfig.FirstOrDefault().Hostname, _ldapConfig.FirstOrDefault().Port);
                cn.Bind(String.Format("stefanini-dom\\{0}", "almintegration"), "stefanini@10");
                var filterInformacaoUsuario = string.Empty;

                foreach (var login in logins)
                {
                    filterInformacaoUsuario = "(sAMAccountName=" + login + ")";
                    var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);
                    if (search.hasMore())
                    {
                        try
                        {
                            var nextEntry = search.next();
                            var user = nextEntry.getAttributeSet();

                            emails.Add(user.getAttribute("mail").StringValue);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                return Ok(emails);
            }
        }

        [HttpGet("buscar-email/{cpf}")]
        [MapToApiVersion("2")]
        [AllowAnonymous]
        public IActionResult BuscarLoginPorCpf([FromRoute] string cpf)
        {
            var idCelula = string.Empty;
            var login = string.Empty;
            using (var cn = new LdapConnection())
            {
                cn.Connect(_ldapConfig.First().Hostname, _ldapConfig.First().Port);
                cn.Bind("stefanini-dom\\almintegration", "stefanini@10");

                LdapSearchConstraints cons = cn.SearchConstraints;
                cons.ReferralFollowing = true;
                cn.Constraints = cons;

                var filterInformacaoUsuario = $"(employeenumber={cpf})";
                var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false, (LdapSearchConstraints)null);

                while (search.hasMore())
                {
                    var nextEntry = search.next();
                    var user = nextEntry.getAttributeSet();

                    idCelula = user.getAttribute("department").StringValue;
                    login = user.getAttribute("sAMAccountName").StringValue;
                    break;
                }
            }

            return Ok(new {idCelula, login});
        }

        #region Metodos privados
        private dynamic ObterPerfisUsuario(string login)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/UsuarioPerfil/obter-funcionalidades/" + login).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //notification handler
                return null;
            }
            else
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<dynamic>(jsonString);
                return result;
            }
        }

        private List<int> ObterCelulasVisualizadas(string login, int celulaUsuario = -1)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(_microServicosUrls.UrlApiControle); 
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var urn = $"api/VisualizacaoCelula/celulas-visualizadas/{login}";
                urn = celulaUsuario > -1 ? urn + $"?celulaUsuario={celulaUsuario}" : urn;
                var response = client.GetAsync(urn).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new List<int>();
                }
                else
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<List<int>>(jsonString);
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string nomeCompleto, string email)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(nomeCompleto))
                return await Task.FromResult<ClaimsIdentity>(null);

            return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, nomeCompleto));
        }

        private async Task<string> GerarTokenLoginUsuario(string userName)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            var key = _configuration["Keys:ChaveLogin"];
            Byte[] textBytes = encoding.GetBytes(userName);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
            {
                hashBytes = hash.ComputeHash(textBytes);
            }

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string TratarCelula(string celulaAD)
        {
            var celulaPosEspaco = celulaAD.Split(" ");
            if (celulaPosEspaco.Length > 0)
            {
                var celulaTratada = celulaPosEspaco[1];
                return celulaTratada;
            }
            else
            {
                return "";
            }
        }

        private string BuscarEmailStefanini(int index, string email)
        {
            string result = string.Empty;

            using (var cn = new LdapConnection())
            {
                try
                {
                    if (index <= _ldapConfig.Length - 1)
                    {
                        cn.Disconnect();
                        cn.Connect(_ldapConfig[index].Hostname, _ldapConfig[index].Port);
                        if (cn.Connected)
                        {
                            cn.Bind(String.Format(_ldapConfig[index].Login), _ldapConfig[index].Password);

                            StringBuilder filterInformacaoUsuario = new StringBuilder();

                            filterInformacaoUsuario.Append("(|");

                            foreach (var searchEmail in _ldapConfig[index].SearchEmails)
                            {
                                filterInformacaoUsuario.Append("(" + searchEmail + "=" + email + ")");
                            }
                            filterInformacaoUsuario.Append(")");

                            var search = cn.Search(_ldapConfig[index].Search_Base, LdapConnection.SCOPE_SUB, filterInformacaoUsuario.ToString(), null, false);

                            if (search.hasMore())
                            {
                                var nextEntry = search.next();
                                var user = nextEntry.getAttributeSet();
                                result = (user.getAttribute("userPrincipalName") != null) ? user.getAttribute("userPrincipalName").StringValue : "";
                            }
                        }
                    }
                }
                catch (LdapException e)
                {
                    return "";
                }
                catch (Exception e)
                {
                    throw;
                }
                return result;
            }
        }

        private string ConectarDAP(int index, string email, string password)
        {
            string userName = "", nomeCompleto = "", cel = "", uidNumber = "", celTratada = "";
            StringBuilder result = new StringBuilder();

            using (var cn = new LdapConnection())
            {
                try
                {
                    if (index <= _ldapConfig.Length - 1)
                    {
                        cn.Disconnect();
                        cn.Connect(_ldapConfig[index].Hostname, _ldapConfig[index].Port);
                        if (cn.Connected)
                        {
                            cn.Bind(String.Format(email), password);

                            StringBuilder filterInformacaoUsuario = new StringBuilder();

                            filterInformacaoUsuario.Append("(|");

                            foreach (var searchEmail in _ldapConfig[index].SearchEmails)
                            {
                                filterInformacaoUsuario.Append("(" + searchEmail + "=" + email + ")");
                            }
                            filterInformacaoUsuario.Append(")");

                            var search = cn.Search(_ldapConfig[index].Search_Base, LdapConnection.SCOPE_SUB, filterInformacaoUsuario.ToString(), null, false);

                            if (search.hasMore())
                            {
                                var nextEntry = search.next();
                                var user = nextEntry.getAttributeSet();
                                userName = (user.getAttribute("sAMAccountName") != null) ? user.getAttribute("sAMAccountName").StringValue : "";
                                nomeCompleto = (user.getAttribute("displayName") != null) ? user.getAttribute("displayName").StringValue : "";
                                cel = (user.getAttribute("department") != null) ? user.getAttribute("department").StringValue.Split('|')[0] : "";
                                celTratada = (user.getAttribute("department") != null) ? TratarCelula(user.getAttribute("department").StringValue.Split('|')[0]) : "";
                                uidNumber = (user.getAttribute("uidNumber") != null) ? user.getAttribute("uidNumber").StringValue : "";
                            }

                            if (userName != "")
                            {
                                result.Append(userName);
                                result.Append(",");
                                result.Append(nomeCompleto);
                                result.Append(",");
                                result.Append(cel);
                                result.Append(",");
                                result.Append(celTratada);
                                result.Append(",");
                                result.Append(uidNumber);
                            }
                        }
                    }
                }
                catch (LdapException e)
                {
                    return "";
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return result.ToString();
        }
        #endregion Metodos privados
    }
}



