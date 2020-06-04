using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.Interfaces;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;
using Utils.Base;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service
{
    public class UsuarioPerfilService : IUsuarioPerfilService
    {
        protected readonly IUsuarioPerfilRepository _usuarioPerfilRepository;
        protected readonly IFuncionalidadeRepository _funcionalidadeRepository;
        protected readonly IVinculoPerfilFuncionalidadeRepository _vinculoPerfilFuncionalidadeRepository;
        protected readonly IUnitOfWork _unitOfWork;
        private readonly MicroServicosUrls _microServicosUrls;

        protected readonly IVariablesToken _variables;
        public UsuarioPerfilService(IUsuarioPerfilRepository usuarioPerfilRepository,
                                    IUnitOfWork unitOfWork,
                                    IFuncionalidadeRepository funcionalidadeRepository,
                                    MicroServicosUrls microServicosUrls,
                                    IVinculoPerfilFuncionalidadeRepository vinculoPerfilFuncionalidadeRepository,
                                    IVariablesToken variables)
        {
            _funcionalidadeRepository = funcionalidadeRepository;
            _vinculoPerfilFuncionalidadeRepository = vinculoPerfilFuncionalidadeRepository;
            _usuarioPerfilRepository = usuarioPerfilRepository;
            _unitOfWork = unitOfWork;
            _microServicosUrls = microServicosUrls;

            _variables = variables;
        }

        public UsuarioPerfilDto ObterUsuarioPerfilComFuncionalidades(string login)
        {
            var result = _usuarioPerfilRepository.ObterUsuarioPerfilDto(login);
            return result;
        }

        public bool VerificaPrestadorMaster(string login)
        {
            var result = _usuarioPerfilRepository.VerificaPrestadorMaster(login);
            return result;
        }

        public List<String> ObterEmailPorFuncionalidade(string[] funcionalidades)
        {
            List<string> emails = new List<string>();
            var objFuncionalidades = _funcionalidadeRepository.Buscar(x => funcionalidades.Any(y => y.Equals(x.NmFuncionalidade, StringComparison.InvariantCultureIgnoreCase)));
            if (objFuncionalidades != null)
            {
                var idPerfis = _vinculoPerfilFuncionalidadeRepository.Buscar(x => objFuncionalidades.Any(y => y.Id == x.IdFuncionalidade)).Select(x => x.IdPerfil);
                var logins = _usuarioPerfilRepository.Buscar(x => idPerfis.Any(y => y == x.IdPerfil)).GroupBy(x => x.LgUsuario).Select(x => x.Key).ToList();
                emails = ObterEmailUsuariosAdPorLogin(logins);
            }

            return emails;
        }

        private List<string> ObterEmailUsuariosAdPorLogin(List<string> logins)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiSeguranca);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(logins), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/authentication/obter-emails", content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<List<string>>(responseString);
        }

        public FiltroGenericoDto<UsuarioPerfilDto> FiltrarUsuario(FiltroGenericoDto<UsuarioPerfilDto> filtro)
        {
            var result = _usuarioPerfilRepository.FiltrarUsuario(filtro);
            return result;
        }

        public void AtualizarVinculos(List<VinculoUsuarioPerfilDto> vinculos)
        {
            foreach (var vinculo in vinculos)
            {
                var vinculosDB = _usuarioPerfilRepository.Buscar(x => x.LgUsuario == vinculo.Login).ToList();
                var vinculosRemovidos = vinculosDB.Where(x => !vinculo.Perfis.Any(y => y == x.IdPerfil)).ToList();
                _usuarioPerfilRepository.RemoveRange(vinculosRemovidos);
                var vinculosNovos = vinculo.Perfis.Where(x => !vinculosDB.Any(y => y.IdPerfil == x));
                foreach (var usuarioPerfil in vinculosNovos.ToList())
                {
                    var usuarioPerfilPersistir = new UsuarioPerfil { LgUsuario = vinculo.Login, IdPerfil = usuarioPerfil };
                    usuarioPerfilPersistir.DataAlteracao = DateTime.Now;
                    usuarioPerfilPersistir.Usuario = _variables.UserName;
                    _usuarioPerfilRepository.Adicionar(usuarioPerfilPersistir);
                }
            }
            _unitOfWork.Commit();
        }

        public void PersistirVinculos(List<VinculoUsuarioPerfilDto> vinculos)
        {
            foreach (var vinculo in vinculos)
            {
                var vinculosDB = _usuarioPerfilRepository.Buscar(x => x.LgUsuario == vinculo.Login).ToList();
                var vinculosNovos = vinculo.Perfis.Where(x => !vinculosDB.Any(y => y.IdPerfil == x));
                foreach (var usuarioPerfil in vinculosNovos.ToList())
                {
                    var usuarioPerfilPersistir = new UsuarioPerfil { LgUsuario = vinculo.Login, IdPerfil = usuarioPerfil };
                    usuarioPerfilPersistir.DataAlteracao = DateTime.Now;
                    usuarioPerfilPersistir.Usuario = _variables.UserName;
                    _usuarioPerfilRepository.Adicionar(usuarioPerfilPersistir);
                }
            }
            _unitOfWork.Commit();
        }

        public void BuscarUsuarioAdPorLogin(FiltroAdDto filtro)
        {

            string filterInformacaoUsuario = "";
            if (filtro.Login != null && filtro.Celula != null)
            {
                filterInformacaoUsuario = "(&(sAMAccountName=*" + filtro.Login + "*)(department=*" + "81" + "*))";
            }
            using (var cn = new LdapConnection())
            {
                try
                {
                    cn.Connect("10.17.2.4", 389);
                    cn.Bind(String.Format("stefanini-dom\\{0}", "lfdamaceno"), "NogameNolife10");
                    var searchBase = string.Empty;
                    // filterInformacaoUsuario = "(sAMAccountName=*" + filtro.Login + "*)";
                    //string filterUsuarios = "(&(department=CEL " + "081" + "*)(userAccountControl=512))";
                    var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false);
                    if (search.hasMore())
                    {

                        var nextEntry = search.next();
                        var user = nextEntry.getAttributeSet();
                        //nomeCompleto = user.getAttribute("displayName").StringValue;
                        //email = user.getAttribute("mail").StringValue;
                    }
                }
                catch (LdapException e)
                {
                    // return null;
                }
            }
        }

        public UsuarioPerfilDto ObterUsuarioComPerfisPorLogin(string login)
        {
            var result = _usuarioPerfilRepository.ObterUsuarioComTodosPerfis(login);

            return result;
        }

        public FiltroGenericoDto<UsuarioPerfilDto> ObterUsuariosComPerfil(FiltroGenericoDto<UsuarioPerfilDto> filtro)
        {
            var result = _usuarioPerfilRepository.ObterUsuariosComPerfil(filtro);
            return result;
        }

        public void RemoverVinculos(string login)
        {
            var vinculosDB = _usuarioPerfilRepository.Buscar(x => x.LgUsuario == login).ToList();
            if (vinculosDB.Any())
            {
                _usuarioPerfilRepository.RemoveRange(vinculosDB);
            }
            _unitOfWork.Commit();
        }

        public List<string> BuscarUsuariosPorPerfis(int[] idPerfis)
        {
            var result = _usuarioPerfilRepository.BuscarUsuariosPorPerfis(idPerfis);
            return result;
        }
    }
}
