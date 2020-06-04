using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Dapper;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class EmpresaPrestadorService : IEmpresaPrestadorService
    {
        private readonly IEmpresaPrestadorRepository _empresaPrestadorRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public EmpresaPrestadorService(
            IEmpresaPrestadorRepository empresaPrestadorRepository,
            IEmpresaRepository empresaRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork)
        {
            _empresaPrestadorRepository = empresaPrestadorRepository;
            _empresaRepository = empresaRepository;
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
        }

        public Empresa Adicionar(EmpresaPrestador empresaPrestador)
        {
            _empresaPrestadorRepository.Adicionar(empresaPrestador);
            _unitOfWork.Commit();
            return empresaPrestador.Empresa;
        }

        public bool VerificaExisteEmpresaNoRm(string cnpj)
        {
            MaskedTextProvider cnpjComMascara = new MaskedTextProvider(@"00\.000\.000/0000-00");
            cnpjComMascara.Set(cnpj);

            var connectionStringIntegracaoRM = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringIntegracaoRM))
            {
                string sQuery = "SELECT * FROM [dbo].[FCFO] WHERE cgccfo = '" + cnpjComMascara + "'";
                bool? existe = dbConnection.Query<bool?>(sQuery).Any();
                dbConnection.Close();
                return existe.HasValue && existe.Value;
            }
        }

        public bool VerificaExisteEmpresaNoEacesso(string cnpj, int idProfissional, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            string sQuery = "SELECT * FROM [stfcorp].[tblProfissionaisEmpresas] WHERE CNPJ = '" + cnpj + "' and idProfissional = " + idProfissional;
            bool? existe = dbConnection.Query<bool?>(sQuery, null, dbTransaction).Any();
            return existe.HasValue && existe.Value;
        }
    }
}
