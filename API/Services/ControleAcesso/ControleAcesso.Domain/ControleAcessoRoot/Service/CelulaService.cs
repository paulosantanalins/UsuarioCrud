
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.DominioRoot.Repository;
using ControleAcesso.Domain.DominioRoot.Service.Interfaces;
using ControleAcesso.Domain.Interfaces;
using ControleAcesso.Domain.SharedRoot;
using Dapper;
using Logger.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.Extensions;


namespace ControleAcesso.Domain.ControleAcessoRoot.Service
{
    public class CelulaService : ICelulaService
    {
        private readonly ICelulaRepository _celulaRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly IDominioRepository _dominioRepository;
        private readonly ITipoCelulaRepository _tipoCelulaRepository;
        private readonly IVinculoTipoCelulaTipoContabilRepository _vinculoTipoCelulaTipoContabilRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IDominioService _dominioService;
        private readonly IVariablesToken _variables;

        public CelulaService(
            ITipoCelulaRepository tipoCelulaRepository,
            ICelulaRepository celulaRepository,
            IGrupoRepository grupoRepository,
            IVinculoTipoCelulaTipoContabilRepository vinculoTipoCelulaTipoContabilRepository,
            IDominioRepository dominioRepository,
            IDominioService dominioService,
            IOptions<ConnectionStrings> connectionStrings,
            MicroServicosUrls microServicosUrls,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _tipoCelulaRepository = tipoCelulaRepository;
            _celulaRepository = celulaRepository;
            _grupoRepository = grupoRepository;
            _vinculoTipoCelulaTipoContabilRepository = vinculoTipoCelulaTipoContabilRepository;
            _dominioRepository = dominioRepository;
            _connectionStrings = connectionStrings;
            _unitOfWork = unitOfWork;
            _variables = variables;
            _microServicosUrls = microServicosUrls;
            _dominioService = dominioService;
        }

        public void Persistir(Celula celula, string loginResponsavel)
        {
            using (var eacessoDbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            using (var stfCorptran = _unitOfWork.BeginTran())
            {
                eacessoDbConnection.Open();
                var eacessoTran = eacessoDbConnection.BeginTransaction();

                var celulaExiste = ValidarExisteCelula(celula.Id);
                var prestadorAD = ObterUsuarioAd(loginResponsavel);
                var idPessoaResponsavelEAcesso = BuscarIdPrestadorEAcesso(prestadorAD.CPF, eacessoDbConnection, eacessoTran);
                celula.IdPessoaResponsavel = ObterIdPessoaResponsavel(idPessoaResponsavelEAcesso);
                celula.EmailResponsavel = "";
                celula.Status = SharedEnuns.StatusCelula.Ativada.GetHashCode();
                celula.NomeResponsavel = prestadorAD.NomeCompleto;
                celula.DataAlteracao = DateTime.Now;

                if (!celulaExiste) { _celulaRepository.Adicionar(celula); }
                else { _celulaRepository.Update(celula); }

                var domDescricaoTipoContabil = _dominioRepository.BuscarPorId((int)celula.IdTipoContabil);
                var domTipoServicoDelivery = _dominioRepository.BuscarPorId((int)celula.IdTipoServicoDelivery);
                var domDescricaoTipoHierarquia = _dominioRepository.BuscarPorId((int)celula.IdTipoHierarquia);

                _unitOfWork.Commit();

                
                celula = _celulaRepository.BuscarComIncludes(celula.Id);
                try {
                    PersistirCelulaEAcesso(
                      eacessoDbConnection,
                      eacessoTran,
                      celula,
                      idPessoaResponsavelEAcesso,
                      domDescricaoTipoContabil,
                      domTipoServicoDelivery,
                      domDescricaoTipoHierarquia
                      );
                }
                catch(Exception e)
                {

                }
              

                stfCorptran.Commit();
                eacessoTran.Commit();
            }
        }

        private int BuscarIdPrestadorEAcesso(string cpf, IDbConnection connection, IDbTransaction transaction)
        {
            string sQuery = "SELECT ID FROM tblProfissionais WHERE CPF = @Cpf";
            return connection.Query<int>(sQuery, new { Cpf = cpf }, transaction).FirstOrDefault();
        }

        public void Adicionar(Celula celula)
        {
            _celulaRepository.Adicionar(celula);
            _unitOfWork.Commit();
        }

        public Celula ObterUltimaCadastrada() => _celulaRepository.BuscarTodos().OrderByDescending(x => x.Id).FirstOrDefault();

        public void Atualizar(Celula celula)
        {
            _celulaRepository.Update(celula);
            _unitOfWork.Commit();
        }

        public List<Celula> ObterTodas() => _celulaRepository.BuscarTodos().ToList();
        public List<TipoCelula> ObterTodosTiposCelula() => _tipoCelulaRepository.BuscarTodos().ToList();

        public List<VinculoTipoCelulaTipoContabil> ObterTodosTiposCelulatipoContabil()
        {
            var vinculos = _vinculoTipoCelulaTipoContabilRepository.BuscarTiposCelulas();
            return vinculos;
        }


        public FiltroGenericoDto<CelulaDto> FiltrarCelula(FiltroGenericoDto<CelulaDto> filtro)
        {
            var result = _celulaRepository.FiltrarCelula(filtro);
            return result;
        }

        public Celula BuscarPorId(int id)
        {
            var celula = _celulaRepository.BuscarPorId(id);
            return celula;
        }

        public void RealizarMigracaoCelulas()
        {
            List<Celula> celulasEacesso = new List<Celula>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            List<Celula> celulasStfCorp = new List<Celula>();
            var connectionStringSTFCORP = Variables.DefaultConnection;

            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT c.idCelula as Id, c.gerente as DescCelula, c.idcelulasup as IdCelulaSuperior, c.idgrupo as IdGrupo, " +
                                "c.idTipoCelula as IdTipoCelula, c.inativo as Inativa, p.nome as NomeResponsavel, lower(p.email_int) as EmailResponsavel, " +
                                "c.ultimaalteracao as DataUltimaAlteracao, c.idPais as idPais, c.idMoeda as IdMoeda, c.idTipoContab as IdTipoContabil, " +
                                "c.idTipoServicosDelivery as IdTipoServicoDelivery, c.IdHierarquia as IdTipoHierarquia FROM [stfcorp].[tblCelulas] c" +
                                " left join stfcorp.tblprofissionais p on p.id = c.idprofissional";

                celulasEacesso = dbConnection.Query<Celula>(sQuery).AsList();
            }

            using (IDbConnection dbConnection = new SqlConnection(connectionStringSTFCORP))
            {
                string sQuery = "SELECT c.idCelula as Id, c.descCelula as DescCelula, c.idcelulasuperior as IdCelulaSuperior, c.idgrupo as IdGrupo, " +
                                "c.idTipoCelula as IdTipoCelula, c.FLINATIVO as Inativa, c.nmResponsavel as NomeResponsavel, c.nmEmailResponsavel as EmailResponsavel, " +
                                "c.dtultimaalteracao as DataUltimaAlteracao, c.idPais as idPais, c.idMoeda as IdMoeda, c.idTipoContabil as IdTipoContabil, " +
                                "c.idTipoServicoDelivery as IdTipoServicoDelivery FROM tblCelula c;";

                celulasStfCorp = dbConnection.Query<Celula>(sQuery).AsList();
            }

            List<Celula> celulasParaUpdate = celulasEacesso.GroupBy(x => celulasStfCorp.Any(y => y.Id == x.Id)).FirstOrDefault().ToList();
            List<Celula> celulasParaAdd = new List<Celula>();

            if (celulasEacesso.GroupBy(x => celulasStfCorp.Any(y => y.Id == x.Id)).Count() > 1)
            {
                celulasParaAdd = celulasEacesso.GroupBy(x => celulasStfCorp.Any(y => y.Id == x.Id)).LastOrDefault().ToList();
            }

            //Adiciona as que não existem
            if (celulasParaAdd?.Any() == true)
            {
                using (IDbConnection dbConnection = new SqlConnection(connectionStringSTFCORP))
                {
                    int inativa;
                    string dtAlteracao;
                    string idCelulaSuperior;
                    string idGrupo;
                    string idTipoCelula;
                    string idTipoHierarquia;
                    string idPais;
                    string idMoeda;
                    string IdTipoContabil;
                    string IdTipoServicoDelivery;

                    foreach (var celula in celulasParaAdd)
                    {
                        inativa = celula.Inativa ? 1 : 0;
                        dtAlteracao = celula.DataAlteracao.HasValue ? ("'" + celula.DataAlteracao.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'") : "null";
                        idCelulaSuperior = celula.IdCelulaSuperior.HasValue && celula.IdCelulaSuperior.Value != -1 ?
                            celula.IdCelulaSuperior.Value.ToString() : "null";
                        idGrupo = celula.IdGrupo.HasValue ? celula.IdGrupo.Value.ToString() : "null";
                        idTipoCelula = celula.IdTipoCelula.HasValue ? celula.IdTipoCelula.Value.ToString() : "null";
                        idTipoHierarquia = celula.IdTipoHierarquia.HasValue ? celula.IdTipoHierarquia.Value.ToString() : "null";
                        idPais = celula.IdPais.HasValue ? celula.IdPais.Value.ToString() : "null";
                        idMoeda = celula.IdMoeda.HasValue ? celula.IdMoeda.Value.ToString() : "null";
                        IdTipoContabil = celula.IdTipoContabil.HasValue ? celula.IdTipoContabil.Value.ToString() : "null";
                        IdTipoServicoDelivery = celula.IdTipoServicoDelivery.HasValue ? celula.IdTipoServicoDelivery.Value.ToString() : "null";

                        string sQuery = $@"insert into tblcelula (idcelula,desccelula,idcelulasuperior,idgrupo,idtipocelula,flinativo,nmresponsavel,
                            nmemailresponsavel,dtultimaalteracao,idTipoHierarquia,IdPais,IdMoeda,IdTipoContabil,IdTipoServicoDelivery)
                            values ( {celula.Id} ,'{celula.DescCelula}',null,{idGrupo},{idTipoCelula},{inativa},'{celula.NomeResponsavel}',
                                '{celula.EmailResponsavel}',{dtAlteracao},{idTipoHierarquia},{idPais},{idMoeda},{IdTipoContabil},{IdTipoServicoDelivery});";
                        dbConnection.Query(sQuery);
                    }

                    foreach (var celula in celulasParaAdd)
                    {
                        idCelulaSuperior = celula.IdCelulaSuperior.HasValue ? celula.IdCelulaSuperior.Value.ToString() : "null";
                        string sQuery = "update tblcelula set idcelulasuperior = " + idCelulaSuperior + " where idCelula = " + celula.Id + "";
                        dbConnection.Query(sQuery);
                    }
                }
            }

            //Update nas que já existem
            if (celulasParaUpdate?.Any() == true)
            {
                using (IDbConnection dbConnection = new SqlConnection(connectionStringSTFCORP))
                {
                    int inativa;
                    string dtAlteracao;
                    string idGrupo;
                    string idTipoCelula;
                    string idCelulaSuperior;
                    string idTipoHierarquia;
                    string idPais;
                    string idMoeda;
                    string IdTipoContabil;
                    string IdTipoServicoDelivery;

                    foreach (var celula in celulasParaUpdate)
                    {

                        inativa = celula.Inativa ? 1 : 0;
                        dtAlteracao = celula.DataAlteracao.HasValue ? ("'" + celula.DataAlteracao.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'") : "null";
                        idCelulaSuperior = celula.IdCelulaSuperior.HasValue && celula.IdCelulaSuperior.Value != -1 ?
                            celula.IdCelulaSuperior.Value.ToString() : "null";
                        idGrupo = celula.IdGrupo.HasValue ? celula.IdGrupo.Value.ToString() : "null";
                        idTipoCelula = celula.IdTipoCelula.HasValue ? celula.IdTipoCelula.Value.ToString() : "null";
                        idTipoHierarquia = celula.IdTipoHierarquia.HasValue ? celula.IdTipoHierarquia.Value.ToString() : "null";
                        idPais = celula.IdPais.HasValue ? celula.IdPais.Value.ToString() : "null";
                        idMoeda = celula.IdMoeda.HasValue ? celula.IdMoeda.Value.ToString() : "null";
                        IdTipoContabil = celula.IdTipoContabil.HasValue ? celula.IdTipoContabil.Value.ToString() : "null";
                        IdTipoServicoDelivery = celula.IdTipoServicoDelivery.HasValue ? celula.IdTipoServicoDelivery.Value.ToString() : "null";

                        string sQuery = $@"UPDATE tblcelula SET desccelula = '{celula.DescCelula}', idcelulasuperior = {idCelulaSuperior},
                                        idgrupo = {idGrupo},idtipocelula = {idTipoCelula}, flinativo = {(celula.Inativa ? 1 : 0)},nmresponsavel = '{celula.NomeResponsavel}',
                                        nmemailresponsavel= '{celula.EmailResponsavel}', dtultimaalteracao = {dtAlteracao},idTipoHierarquia = {idTipoHierarquia},
                                        idPais = {idPais}, idMoeda = {idMoeda} ,idTipoContabil = {IdTipoContabil}, idTipoServicoDelivery = {IdTipoServicoDelivery}
                                        WHERE idCelula = {celula.Id};";
                        dbConnection.Query(sQuery);
                    }
                }
            }
        }

        public void AtualizarMigracaoCelulas()
        {
            var celulasEacesso = CelulasEAcesso();
            var celulasStfcorp = _celulaRepository.BuscarTodosComInclude();

            var celulasNovas = celulasEacesso.Where(x => !celulasStfcorp.Any(y => y.Id == x.Id));
            AdicionarCelulasSTFCorp(celulasNovas);

            var celulasAntigas = celulasEacesso.Where(x => celulasStfcorp.Any(y => y.Id == x.Id));
            AtualizarCelulasSTFCorp(celulasAntigas);
        }

        private void AdicionarCelulasSTFCorp(IEnumerable<Celula> celulasNovas)
        {
            var connectionStringSTFCORP = Variables.DefaultConnection;

            using (IDbConnection dbConnection = new SqlConnection(connectionStringSTFCORP))
            {
                try
                {
                    int inativa;
                    string dtAlteracao;
                    string idGrupo;
                    string idTipoCelula;
                    string idCelulaSuperior;
                    string idTipoHierarquia;
                    string idPais;
                    string idMoeda;
                    string idTipoContabil;
                    string IdTipoServicoDelivery;
                    int? idPessoaResponsavel;

                    foreach (var celula in celulasNovas)
                    {
                        inativa = celula.Inativa ? 1 : 0;
                        dtAlteracao = celula.DataAlteracao.HasValue ? ("'" + celula.DataAlteracao.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'") : "null";
                        idGrupo = celula.IdGrupo.HasValue ? celula.IdGrupo.Value.ToString() : "null";
                        idTipoCelula = celula.IdTipoCelula.HasValue ? celula.IdTipoCelula.Value.ToString() : "null";
                        idPais = celula.IdPais.HasValue ? celula.IdPais.Value.ToString() : "null";
                        idMoeda = celula.IdMoeda.HasValue ? celula.IdMoeda.Value.ToString() : "null";

                        idPessoaResponsavel = celula.IdEacesso.HasValue ? ObterIdPessoaResponsavel(celula.IdEacesso) : null;
                        var idPessoaResponsavelTratado = idPessoaResponsavel.HasValue ? idPessoaResponsavel.ToString() : "null";

                        idTipoHierarquia = idTipoHierarquia = celula.IdTipoHierarquia.HasValue ?
                          _dominioService.ObterIdDominio((int)celula.IdTipoHierarquia, SharedEnuns.DominioTipoHierarquia.VL_TIPO_DOMINIO.GetDescription())
                          .ToString() : "null";
                        idTipoContabil = celula.IdTipoContabil.HasValue ? _dominioService.ObterIdDominio((int)celula.IdTipoContabil,
                            SharedEnuns.DominioTipoContabil.VL_TIPO_DOMINIO.GetDescription())
                          .ToString() : "null";
                        IdTipoServicoDelivery = celula.IdTipoServicoDelivery.HasValue ? _dominioService.ObterIdDominio((int)celula.IdTipoContabil,
                            SharedEnuns.DominioTipoServicoDelivery.VL_TIPO_DOMINIO.GetDescription())
                          .ToString() : "null";

                        string sQuery = $@"INSERT  INTO TBLCELULA (idcelula, desccelula, idcelulasuperior, idgrupo, idtipocelula, flinativo, nmresponsavel, nmemailresponsavel,
                        dtUltimaAlteracao, idTipoHierarquia, IdPais, IdMoeda, IdTipoContabil, IdTipoServicoDelivery, idPessoaResponsavel, lgUsuario) 
                        VALUES ({ celula.Id}, '{celula.DescCelula}', null, {idGrupo} , {idTipoCelula}, {inativa} , '{celula.NomeResponsavel}',
                    '{celula.EmailResponsavel}', {dtAlteracao}, {idTipoHierarquia}, {idPais}, {idMoeda}, {idTipoContabil}, {IdTipoServicoDelivery}, {idPessoaResponsavelTratado}, 'STFCORP');";

                        dbConnection.Query(sQuery);
                    }

                    foreach (var celula in celulasNovas)
                    {
                        idCelulaSuperior = celula.IdCelulaSuperior.HasValue ? celula.IdCelulaSuperior.Value.ToString() : "null";
                        string sQuery = "update tblcelula set idcelulasuperior = " + idCelulaSuperior + " where idCelula = " + celula.Id + "";
                        dbConnection.Query(sQuery);
                    }
                }
                catch (Exception ex)
                {
                    dbConnection.Close();
                    throw ex;
                }
            }
        }

        private void AtualizarCelulasSTFCorp(IEnumerable<Celula> celulasAntigas)
        {
            var connectionStringSTFCORP = Variables.DefaultConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringSTFCORP))
            {
                try
                {
                    foreach (var celula in celulasAntigas)
                    {
                        string dtAlteracao = celula.DataAlteracao.HasValue ? ("'" + celula.DataAlteracao.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'") : "null";
                        string idGrupo = celula.IdGrupo.HasValue ? celula.IdGrupo.Value.ToString() : "null";
                        string idTipoCelula = celula.IdTipoCelula.HasValue ? celula.IdTipoCelula.Value.ToString() : "null";
                        string idPais = idPais = celula.IdPais.HasValue ? celula.IdPais.Value.ToString() : "null";
                        string idMoeda = idMoeda = celula.IdMoeda.HasValue ? celula.IdMoeda.Value.ToString() : "null";
                        string idCelulaSuperior = celula.IdCelulaSuperior.HasValue && celula.IdCelulaSuperior.Value >= 0 ? celula.IdCelulaSuperior.Value.ToString() : "null";

                        int? idPessoaResponsavel = celula.IdEacesso.HasValue ? ObterIdPessoaResponsavel(celula.IdEacesso) : null;
                        string idPessoaResponsavelTratado = idPessoaResponsavel.HasValue ? idPessoaResponsavel.ToString() : "null";

                        string idTipoHierarquia = idTipoHierarquia = celula.IdTipoHierarquia.HasValue ?
                          _dominioService.ObterIdDominio((int)celula.IdTipoHierarquia, SharedEnuns.DominioTipoHierarquia.VL_TIPO_DOMINIO.GetDescription())
                          .ToString() : "null";
                        string idTipoContabil = celula.IdTipoContabil.HasValue ? _dominioService.ObterIdDominio((int)celula.IdTipoContabil,
                            SharedEnuns.DominioTipoContabil.VL_TIPO_DOMINIO.GetDescription())
                          .ToString() : "null";
                        string IdTipoServicoDelivery = celula.IdTipoServicoDelivery.HasValue ? _dominioService.ObterIdDominio((int)celula.IdTipoServicoDelivery,
                            SharedEnuns.DominioTipoServicoDelivery.VL_TIPO_DOMINIO.GetDescription())
                          .ToString() : "null";

                        string sQuery = $@"UPDATE tblcelula SET desccelula = '{celula.DescCelula}', idcelulasuperior = {idCelulaSuperior},
                                        idgrupo = {idGrupo}, idtipocelula = {idTipoCelula}, flinativo = {(celula.Inativa ? 1 : 0)}, nmresponsavel = '{celula.NomeResponsavel}',
                                        idPessoaResponsavel = {idPessoaResponsavelTratado}, nmemailresponsavel= '{celula.EmailResponsavel}',
                                        dtUltimaAlteracao = {dtAlteracao}, idTipoHierarquia = {idTipoHierarquia},
                                        idPais = {idPais}, idMoeda = {idMoeda}, idTipoContabil = {idTipoContabil}, idTipoServicoDelivery = {IdTipoServicoDelivery}, lgUsuario = 'STFCORP' 
                                        WHERE idCelula = {celula.Id};";

                        dbConnection.Query(sQuery);
                    }
                }
                catch (Exception ex)
                {
                    dbConnection.Close();
                    throw ex;
                }

            }
        }
        public List<Celula> BuscarCelulasQuePessoaEhResponsavel(int idPessoa)
        {
            var celulas = _celulaRepository.Buscar(x => x.IdPessoaResponsavel == idPessoa).ToList();
            return celulas;
        }


        private List<Celula> CelulasEAcesso()
        {
            List<Celula> celulasEacesso;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT c.idCelula as Id, c.gerente as DescCelula, c.idcelulasup as IdCelulaSuperior, c.idgrupo as IdGrupo, c.idprofissional as idEacesso," +
                                "c.idTipoCelula as IdTipoCelula, c.inativo as Inativa, p.nome as NomeResponsavel, lower(p.email_int) as EmailResponsavel, " +
                                "c.ultimaalteracao as DataUltimaAlteracao, c.idHierarquia as IdTipoHierarquia, c.idPais as IdPais, c.idMoeda as IdMoeda," +
                                "c.idTipoContab as IdTipoContabil, c.idTipoServicosDelivery as IdTipoServicoDelivery" +
                                " FROM [stfcorp].[tblCelulas] c left join stfcorp.tblprofissionais p on p.id = c.idprofissional";

                dbConnection.Open();
                celulasEacesso = dbConnection.Query<Celula>(sQuery).AsList();
                dbConnection.Close();
            }

            return celulasEacesso;
        }

        private int? ObterIdPessoaResponsavel(int? idEacesso)
        {
            if (!idEacesso.HasValue)
            {
                return null;
            }

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(120),
                BaseAddress = new Uri(_microServicosUrls.UrlApiCadastro)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync("api/Pessoa/idEacesso/" + idEacesso).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            if (responseString != null)
            {
                return Int32.Parse(responseString);
            }
            return null;

        }

        private int ObterIdTipoContabilEacesso(string tipoDominio, string valorDominio)
        {
            int id = _dominioService.ObterIdDominio(tipoDominio, valorDominio);
            return id;
        }

        private int ObterIdTipoServicoDeliveryEacesso(string tipoDominio, string valorDominio)
        {
            int id = _dominioService.ObterIdDominio(tipoDominio, valorDominio);
            return id;
        }

        public string ObterEmailGerenteServico(int idCelula)
        {
            var emailGS = _celulaRepository.ObterEmailGerenteServico(idCelula);
            return emailGS;
        }

        public void Inativar(int id)
        {
            VerififcarSeCelulaTemPendenciasParaInativacao(id);

            using (var eacessoDbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                eacessoDbConnection.Open();
                var eacessoTran = eacessoDbConnection.BeginTransaction();

                try
                {
                    var model = _celulaRepository.BuscarPorId(id);

                    if (ExisteServicoPendente(id))
                    {
                        model.Status = SharedEnuns.StatusCelula.PendenteParaInativacao.GetHashCode();
                    }
                    else
                    {
                        model.Status = SharedEnuns.StatusCelula.Inativada.GetHashCode();
                        InativarReativarCelulaEacesso(id, eacessoDbConnection, eacessoTran, true);
                    }

                    _unitOfWork.Commit();
                    eacessoTran.Commit();
                }
                catch (Exception)
                {
                    eacessoTran.Rollback();
                    throw;
                }
                finally
                {
                    eacessoDbConnection.Close();
                }
            }
        }

        public void RealizarInativacoesJob()
        {
            var celulasAInativar = _celulaRepository.Buscar(x =>
                x.Status == SharedEnuns.StatusCelula.PendenteParaInativacao.GetHashCode());

            foreach (var celula in celulasAInativar)
            {
                try
                {
                    if (!ExisteServicoPendente(celula.Id))
                    {
                        Inativar(celula.Id);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static void InativarReativarCelulaEacesso(int id, SqlConnection eacessoDbConnection, SqlTransaction eacessoTran, bool inativar)
        {
            var query = $"update tblCelulas set Fl_Inativo = {(inativar ? "1" : "0")} where IdCelula = {id};";

            var command = new SqlCommand(query, eacessoDbConnection, eacessoTran);

            command.ExecuteNonQuery();
        }

        private void VerififcarSeCelulaTemPendenciasParaInativacao(int idCelula)
        {
            if (VerificarSeCelulaTemCelulasSubordinadasNoEAcesso(idCelula) > 0) { throw new Exception("Célula não pode ser inativada pois possui células subordinadas"); }
        }

        private int VerificarSeCelulaTemCelulasSubordinadasNoEAcesso(int idCelula)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                dbConnection.Open();
                string sQuery = "SELECT COUNT(*) FROM [stfcorp].[tblCelulas] WHERE IdCelulaSup = @IdCelula";
                int count = dbConnection.Execute(sQuery, new { IdCelula = idCelula });
                return count;
            }
        }

        public void Reativar(int id)
        {
            using (var eacessoDbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                eacessoDbConnection.Open();
                var eacessoTran = eacessoDbConnection.BeginTransaction();

                try
                {
                    var model = _celulaRepository.BuscarPorId(id);

                    model.Status = SharedEnuns.StatusCelula.Reativada.GetHashCode();

                    InativarReativarCelulaEacesso(id, eacessoDbConnection, eacessoTran, false);

                    _unitOfWork.Commit();
                    eacessoTran.Commit();
                }
                catch (Exception)
                {
                    eacessoTran.Rollback();
                    throw;
                }
                finally
                {
                    eacessoDbConnection.Close();
                }
            }
        }

        public bool ValidarExisteCelula(int id)
        {
            return _celulaRepository.ValidarExisteCelula(id);
        }

        public bool ExisteServicoPendente(int idCelula)
        {
            using (var eacessoDbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                eacessoDbConnection.Open();
                var query = $@"SELECT Count(idServico)
                                FROM stfcorp.tblClientesServicos
                                INNER JOIN TBLCLIENTES C ON tblClientesServicos.IDCLIENTE = C.IDCLIENTE
                                INNER JOIN stfcorp.tblCelulas Cel ON tblClientesServicos.idCelula = Cel.IdCelula
                                WHERE tblClientesServicos.idCelula= {idCelula}";

                var result = eacessoDbConnection.QueryFirst<int>(query);

                return result > 0;
            }
        }

        public FiltroGenericoDto<ServicoCelulaDTO> BuscarServicosPendente(FiltroGenericoDto<ServicoCelulaDTO> filtro)
        {
            using (var eacessoDbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                eacessoDbConnection.Open();

                var query = $@" SELECT NomeFantasia,cs.IdCliente
								FROM STFCORP.tblClientes
								inner join STFCORP.tblClientesServicos cs on cs.IdCliente = STFCORP.tblClientes.IdCliente
                                WHERE cs.idCelula={filtro.ValorParaFiltrar}
								group by nomeFantasia,cs.IdCliente";
                var servicos = eacessoDbConnection.Query<ServicoCelulaDTO>(query).AsList();

                servicos.ForEach(x =>
                {
                    var query2 = $@" SELECT Nome,Idservico
								FROM stfcorp.tblClientesServicos
                                INNER JOIN stfcorp.tblCelulas Cel ON tblClientesServicos.idCelula = Cel.IdCelula
								inner join STFCORP.tblClientes cliente on tblClientesServicos.IdCliente = cliente.IdCliente
                                WHERE tblClientesServicos.IdCliente= {x.IdCliente}";

                    x.servicos = eacessoDbConnection.Query<ServicosPendentesClienteDTO>(query2).AsList();

                });
                filtro.Total = servicos.Count();
                filtro.Valores = servicos.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
                return filtro;
            }
        }

        private void PersistirCelulaEAcesso(
             IDbConnection dbConnection,
             IDbTransaction dbTransaction,
             Celula celula,
             int idPessoaResponsavelEAcesso,
             Dominio domTipoContabil,
             Dominio domTipoServicoDelivery,
             Dominio domTipoHierarquia
             )
        {
            var query = "";
            if (!ExisteCelulaNoEacesso(celula.Id))
            {
                var siglaPais = ObterSiglaPaisPorId(celula.IdPais);
                query = $@"SET IDENTITY_INSERT STFCORP.TBLCELULAS ON;

                        INSERT [stfcorp].[tblCelulas] (IdCelula, Gerente, IdProfissional, IdGrupo, IdTipoCelula, IdTipoContab, IdMoeda, Inativo, IdCelulaSup,
                        IdHierarquia, IdPais, Descricao, IdTipoServicosDelivery, UltimaAlteracao, CustoNominaPercent, CustoNominaEstagP,
                        BenefIntegral, AprovContratDiretor, FlagSecaoHierarquia, AprovSolic_NF, ExteriorClave, ExteriorTipoDescanso,
                        ExteriorTipoFerCalculo, LancaHRRM, RepasseEPM, RepasseMesmaCel, RateioIVA)
                        VALUES (@IdCelula, @Gerente, @IdProfissional, @IdGrupo, @IdTipoCelula,
                        (select IdTipoContab from [stfcorp].[tblTiposContab] WHERE Descricao = @DescricaoTipoContab),
                        @IdMoeda , @Inativo , @IdCelulaSup, @IdHierarquia,
                        (select idPais from [stfcorp].[tblPaises] WHERE SiglaPais = @SiglaPais),
                        @Descricao, (select id from [stfcorp].[tblTiposServicosDelivery] 
                        WHERE Descricao = @DescricaoTipoServicoDelivery), @UltimaAlteracao, @CustoNominaPercent, @CustoNominaEstagP, @BenefIntegral,
                        @AprovContratDiretor, @FlagSecaoHierarquia, @AprovSolic_NF, @ExteriorClave, @ExteriorTipoDescanso, @ExteriorTipoFerCalculo,
                        @LancaHRRM, @RepasseEPM, @RepasseMesmaCel, @RateioIVA);

                        SET IDENTITY_INSERT STFCORP.TBLCELULAS OFF;";

                dbConnection.Execute(query, new
                {

                    IdCelula = celula.Id,
                    Gerente = celula.DescCelula,
                    IdProfissional = idPessoaResponsavelEAcesso,
                    IdGrupo = celula.IdGrupo,
                    IdTipoCelula = celula.IdTipoCelula,
                    DescricaoTipoContab = domTipoContabil.DescricaoValor,
                    IdMoeda = celula.IdMoeda,
                    Inativo = Convert.ToInt32(celula.Status),
                    IdCelulaSup = celula.IdCelulaSuperior,
                    IdHierarquia = domTipoHierarquia.IdValor,
                    SiglaPais = siglaPais,
                    Descricao = celula.DescCelula,
                    DescricaoTipoServicoDelivery = domTipoServicoDelivery.DescricaoValor,
                    UltimaAlteracao = celula.DataAlteracao ?? (object)DBNull.Value,
                    CustoNominaPercent = 0,
                    CustoNominaEstagP = 0,
                    BenefIntegral = 0,
                    AprovContratDiretor = 0,
                    FlagSecaoHierarquia = 0,
                    AprovSolic_NF = 0,
                    ExteriorClave = 0,
                    ExteriorTipoDescanso = 0,
                    ExteriorTipoFerCalculo = 0,
                    LancaHRRM = 0,
                    RepasseMesmaCel = celula.FlHabilitarRepasseMesmaCelula,
                    RepasseEPM = celula.FlHabilitarRepasseEpm,
                    RateioIVA = 1
                }, transaction: dbTransaction);
            }
            else
            {
                var siglaPais = ObterSiglaPaisPorId(celula.IdPais);
                query = $@"UPDATE [stfcorp].[tblCelulas] SET
                                        Gerente = '{celula.DescCelula}', 
                                        IdProfissional = {celula.Pessoa.IdEacesso},
                                        IdGrupo = {celula.IdGrupo},
                                        IdTipoCelula = {celula.IdTipoCelula},
                                        IdTipoContab = (select IdTipoContab from [stfcorp].[tblTiposContab] WHERE Descricao = '{celula.TipoContabil.DescricaoValor}'),
                                        IdMoeda = {celula.IdMoeda}, 
                                        Inativo = {Convert.ToInt32(celula.Status)},
                                        IdCelulaSup = {celula.IdCelulaSuperior},
                                        IdHierarquia = {celula.TipoHierarquia.IdValor}, 
                                        IdPais = (select idPais from [stfcorp].[tblPaises] WHERE SiglaPais = '{siglaPais}'),
                                        Descricao = '{celula.DescCelula}', 
                                        IdTipoServicosDelivery = (select id from [stfcorp].[tblTiposServicosDelivery] WHERE Descricao = '{celula.TipoServicoDelivery.DescricaoValor}'),
                                        UltimaAlteracao = @UltimaAlteracao,
                                        RepasseEPM = @RepasseEPM,
                                        RepasseMesmaCel = @RepasseMesmaCel
                                        WHERE idCelula = {celula.Id}";
                dbConnection.Query(query, new { UltimaAlteracao = celula?.DataAlteracao ?? (object)DBNull.Value, RepasseEPM = celula.FlHabilitarRepasseEpm, RepasseMesmaCel = celula.FlHabilitarRepasseMesmaCelula }, transaction: dbTransaction);
            }
        }

        private string TratarApostofre(string texto)
        {
            if (texto != null)
            {
                texto = texto.Replace("'", "''");
            }
            return texto;
        }

        public bool ExisteCelulaNoEacesso(int idCelula)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                string sQuery = "SELECT * FROM [stfcorp].[tblCelulas] WHERE IdCelula = '" + idCelula + "'";
                bool? existe = dbConnection.Query<bool?>(sQuery).Any();
                return existe.HasValue && existe.Value;
            }
        }

        public UsuarioAdDto ObterUsuarioAd(string login)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", _variables.Token);

                    client.BaseAddress = new Uri(_microServicosUrls.UrlApiSeguranca);

                    var result = client.GetAsync("api/authentication/FiltrarUsuariosADPorLogin/" + login).Result;

                    var content = result.Content.ReadAsStringAsync().Result;
                    var usuario = JsonConvert.DeserializeObject<IEnumerable<UsuarioAdDto>>(content);
                    return usuario.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var x = _variables.Token;
                throw ex;
            }


        }

        public (int, string) ObterIdCelulaResponsavel(string cpf)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", _variables.Token);

                client.BaseAddress = new Uri(_microServicosUrls.UrlApiSeguranca);
                var result = client.GetAsync($"api/authentication/buscar-email/{cpf}").Result;

                var content = result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ResponseResponsavelCelula>(content);
                var celula = Convert.ToInt32(response.IdCelula.Split(' ')[1]);
                return (celula, response.Login);
            }
        }

        private int? ObterIdPessoa(int? idEacesso)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_microServicosUrls.UrlApiCadastro);
                    var result = client.GetAsync("api/pessoa/idEacesso/" + idEacesso).Result;
                    string content = "";
                    int? id = null;

                    if (result.Content != null)
                    {
                        content = result.Content.ReadAsStringAsync().Result;
                        id = JsonConvert.DeserializeObject<int>(content);
                    }

                    return id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Pessoa ObterPessoaApiCadastro(int? id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_microServicosUrls.UrlApiCadastro);
                    var result = client.GetAsync("api/pessoa/obter-por-id/" + id).Result;


                    var content = result.Content.ReadAsStringAsync().Result;
                    var pessoa = JsonConvert.DeserializeObject<Pessoa>(content);
                    return pessoa;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ObterSiglaPaisPorId(int? idPais)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_microServicosUrls.UrlApiCadastro);
                    var result = client.GetAsync($"api/endereco/siglaPaisPorIdPais?idPais={idPais}").Result;

                    var content = result.Content.ReadAsStringAsync().Result;
                    var sigla = JsonConvert.DeserializeObject<string>(content);
                    return sigla;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<LogCampoCelulaDto>> ObterLogsDeAlteracaoDeCampoDeCelulaPorCelula(int idCelula)
        {
            var listaAuditoria = await _celulaRepository.BuscarListaDeAuditoriaDeCelula(idCelula);
            listaAuditoria = listaAuditoria.OrderByDescending(x => x.DataAlteracao).ToList();
            List<LogCampoCelulaDto> logList = new List<LogCampoCelulaDto>();

            foreach (var audit in listaAuditoria)
            {

                if (audit.ValoresAntigos != null)
                {
                    logList = logList.Concat(ObterLogsDeAlteracaoDeCampoDeCelulaPorAuditoria(audit, idCelula)).ToList();
                }
            }

            return logList;
        }

        private List<LogCampoCelulaDto> ObterLogsDeAlteracaoDeCampoDeCelulaPorAuditoria(Auditoria audit, int idCelula)
        {
            List<LogCampoCelulaDto> logList = new List<LogCampoCelulaDto>();
            var valoresAnteriores = JObject.Parse(audit.ValoresAntigos);
            var valoresNovos = JObject.Parse(audit.ValoresNovos);
            IList<string> keys = valoresNovos.Properties().Select(p => p.Name).ToList();

            using (var client = new HttpClient())
            {
                foreach (var key in keys)
                {
                    if (AtributoDeLogDeAuditoriaEhCampoDeCelula(key))
                    {
                        logList.Add(new LogCampoCelulaDto
                        {
                            Campo = ObterNomeDoCampoAlterado(key),
                            ValorAnterior = ObterDescricaoDeLogCampoCelula(client, key, valoresAnteriores[key].ToString()),
                            ValorNovo = ObterDescricaoDeLogCampoCelula(client, key, valoresNovos[key].ToString()),
                            Usuario = audit.Usuario,
                            DtAlteracao = audit.DataAlteracao
                        });

                        if (ObterNomeDoCampoAlterado(key).Equals("RESPONSÁVEL"))
                        {
                            /* Solução momentânea para aparição de Célula Responsável */

                            var cpfAntigo = ObterPessoaApiCadastro(Convert.ToInt32(valoresAnteriores[key].ToString())).Cpf;

                            var cpfNovo = ObterPessoaApiCadastro(Convert.ToInt32(valoresNovos[key].ToString())).Cpf;

                            var (IdCelulaResponsavelAntiga, login) = ObterIdCelulaResponsavel(cpfAntigo);
                            var (IdCelulaResponsavelNova, loginCelulaNova) = ObterIdCelulaResponsavel(cpfNovo);

                            if (IdCelulaResponsavelNova != IdCelulaResponsavelAntiga)
                            {
                                LogCampoCelulaDto logCelula = new LogCampoCelulaDto();

                                logCelula.Campo = "CÉLULA RESPONSÁVEL";
                                logCelula.DtAlteracao = audit.DataAlteracao;
                                logCelula.Usuario = audit.Usuario;

                                logCelula.ValorNovo = IdCelulaResponsavelNova.ToString();
                                logCelula.ValorAnterior = IdCelulaResponsavelAntiga.ToString();

                                logList.Add(logCelula);
                            }

                        }

                    }
                }
            }

            return logList;
        }

        private bool AtributoDeLogDeAuditoriaEhCampoDeCelula(string key)
        {
            var colunasDosCampos = new List<string>
            {
                "IDCELULA","DESCCELULA","IDPAIS","IDMOEDA","IDPESSOARESPONSAVEL","IDTIPOHIERARQUIA","IDTIPOCELULA","IDTIPOCONTABIL",
                "IDGRUPO","IDCELULASUPERIOR","IDTIPOSERVICODELIVERY","IDEMPRESAGRUPO","FLHABILITARREPASSEMESMACELULA","FLHABILITARREPASSEEPM","STATUS"
             };

            return colunasDosCampos.Contains(key.ToUpper());
        }

        private string ObterDescricaoDeLogCampoCelula(HttpClient client, string key, string value)
        {
            switch (key.ToUpper())
            {
                case "IDPAIS":
                    var paisResult = !string.IsNullOrWhiteSpace(value) ? client.GetAsync($"{_microServicosUrls.UrlApiCadastro}api/cidade/pais/{value}")?.Result : null;
                    var paisNome = paisResult != null ? JsonConvert.DeserializeObject<PaisDto>(paisResult.Content.ReadAsStringAsync()?.Result)?.NmPais : null;
                    return paisNome;
                case "IDMOEDA":
                    var moedaResult = !string.IsNullOrWhiteSpace(value) ? client.GetAsync($"{_microServicosUrls.UrlApiServico}api/moeda/{value}")?.Result : null;
                    var moedaDescricao = moedaResult != null ? JsonConvert.DeserializeObject<MoedaDto>(moedaResult.Content.ReadAsStringAsync()?.Result)?.Nome : null;
                    return moedaDescricao;
                case "IDEMPRESAGRUPO":
                    var empresaResult = !string.IsNullOrWhiteSpace(value) ? client.GetAsync($"{_microServicosUrls.UrlApiCadastro}api/empresaGrupo/rm/{value}")?.Result : null;
                    var descricao = empresaResult != null ? JsonConvert.DeserializeObject<EmpresaGrupoDto>(empresaResult.Content.ReadAsStringAsync()?.Result)?.Descricao : null;
                    return descricao;
                case "IDCELULASUPERIOR":
                    var celulaSuperior = _celulaRepository.BuscarPorId(Convert.ToInt32(value))?.Id;
                    return celulaSuperior.ToString();
                case "IDGRUPO":
                    var grupo = _grupoRepository.BuscarPorId(Convert.ToInt32(value))?.DescGrupo;
                    return grupo;
                case "IDPESSOARESPONSAVEL":
                    var pessoaResult = !string.IsNullOrWhiteSpace(value) ? client.GetAsync($"{_microServicosUrls.UrlApiCadastro}api/pessoa/obter-por-id/{value}")?.Result : null;
                    var nomePessoa = pessoaResult != null ? JsonConvert.DeserializeObject<PessoaDto>(pessoaResult.Content.ReadAsStringAsync()?.Result)?.Nome : null;
                    return nomePessoa;
                case "IDTIPOCELULA":
                    var tipoCelula = _tipoCelulaRepository.BuscarPorId(Convert.ToInt32(value))?.Descricao;
                    return tipoCelula;
                case "IDTIPOCONTABIL":
                    var tipoContabil = !string.IsNullOrWhiteSpace(value) ? _dominioRepository.BuscarPorId(Convert.ToInt32(value))?.DescricaoValor : null;
                    return tipoContabil;
                case "IDTIPOSERVICODELIVERY":
                    var tipoServicoDelivery = !string.IsNullOrWhiteSpace(value) ? _dominioRepository.BuscarPorId(Convert.ToInt32(value))?.DescricaoValor : null;
                    return tipoServicoDelivery;
                case "IDTIPOHIERARQUIA":
                    var tipoHierarquia = !string.IsNullOrWhiteSpace(value) ? _dominioRepository.BuscarPorId(Convert.ToInt32(value))?.DescricaoValor : null;
                    return tipoHierarquia;
                case "FLHABILITARREPASSEMESMACELULA":
                    return value.Equals("True") ? "HABILITADO" : "DESABILITADO";
                case "FLHABILITARREPASSEEPM":
                    return value.Equals("True") ? "HABILITADO" : "DESABILITADO";
                case "STATUS":
                    return ObterDescricaoStatusCelula(value);
                default:
                    return value;
            }
        }

        private string ObterNomeDoCampoAlterado(string key)
        {
            switch (key.ToUpper())
            {
                case "IDCELULA":
                    return SharedEnuns.NomeCamposCadastroCelula.NUMERO.GetDescription();
                case "IDCELULASUPERIOR":
                    return SharedEnuns.NomeCamposCadastroCelula.CELULA_SUPERIOR.GetDescription();
                case "DESCCELULA":
                    return SharedEnuns.NomeCamposCadastroCelula.DESCRICAO.GetDescription();
                case "IDGRUPO":
                    return SharedEnuns.NomeCamposCadastroCelula.GRUPO.GetDescription();
                case "IDTIPOCELULA":
                    return SharedEnuns.NomeCamposCadastroCelula.TIPO_CELULA.GetDescription();
                case "IDPESSOARESPONSAVEL":
                    return SharedEnuns.NomeCamposCadastroCelula.PESSOA_RESPONSAVEL.GetDescription();
                case "IDTIPOHIERARQUIA":
                    return SharedEnuns.NomeCamposCadastroCelula.TIPO_HIERARQUIA.GetDescription();
                case "IDPAIS":
                    return SharedEnuns.NomeCamposCadastroCelula.PAIS.GetDescription();
                case "IDMOEDA":
                    return SharedEnuns.NomeCamposCadastroCelula.MOEDA.GetDescription();
                case "IDTIPOCONTABIL":
                    return SharedEnuns.NomeCamposCadastroCelula.TIPO_CONTABIL.GetDescription();
                case "IDTIPOSERVICODELIVERY":
                    return SharedEnuns.NomeCamposCadastroCelula.TIPO_SERVICO_DELIVERY.GetDescription();
                case "IDEMPRESAGRUPO":
                    return SharedEnuns.NomeCamposCadastroCelula.EMPRESA_GRUPO.GetDescription();
                case "FLHABILITARREPASSEMESMACELULA":
                    return SharedEnuns.NomeCamposCadastroCelula.REPASSE_CELULA.GetDescription();
                case "FLHABILITARREPASSEEPM":
                    return SharedEnuns.NomeCamposCadastroCelula.REPASSE_EPM.GetDescription();
                case "STATUS":
                    return SharedEnuns.NomeCamposCadastroCelula.STATUS.GetDescription();
                default:
                    return "";
            }
        }

        private string ObterDescricaoStatusCelula(string valor)
        {
            switch (valor)
            {
                case "0":
                    return SharedEnuns.StatusCelula.Ativada.GetDescription();
                case "1":
                    return SharedEnuns.StatusCelula.Inativada.GetDescription();
                case "2":
                    return SharedEnuns.StatusCelula.PendenteParaInativacao.GetDescription();
                case "3":
                    return SharedEnuns.StatusCelula.Reativada.GetDescription();
                default:
                    return string.Empty;
            }
        }

        public List<Celula> ObterTodasEacesso()
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            List<Celula> celulasEacesso = new List<Celula>();

            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "SELECT c.idCelula as Id, c.gerente as DescCelula FROM [stfcorp].[tblCelulas] c";
                celulasEacesso = dbConnection.Query<Celula>(sQuery).AsList();
            }

            return celulasEacesso;
        }
    }
}
