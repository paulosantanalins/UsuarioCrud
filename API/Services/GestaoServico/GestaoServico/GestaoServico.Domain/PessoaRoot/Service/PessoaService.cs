using Dapper;
using GestaoServico.Domain.Interfaces;
using GestaoServico.Domain.PessoaRoot.Entity;
using GestaoServico.Domain.PessoaRoot.Repository;
using GestaoServico.Domain.PessoaRoot.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Connections;

namespace GestaoServico.Domain.PessoaRoot.Service
{
    public class PessoaService : IPessoaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public PessoaService(
            IPessoaRepository pessoaRepository,
            IOptions<ConnectionStrings> connectionStrings,
            IUnitOfWork unitOfWork)
        {
            _pessoaRepository = pessoaRepository;
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
        }

        public void Migrar()
        {
            List<Pessoa> pessoas = ObterPessoasEAcesso();
            PersistirPessoas(pessoas);
        }

        private void PersistirPessoas(List<Pessoa> pessoas)
        {
            int contador = 0;
            foreach (var pessoa in pessoas)
            {
                _pessoaRepository.Adicionar(pessoa);
                contador++;
                if (contador == 100)
                {
                    _unitOfWork.Commit();
                    contador = 0;
                }
            }
            if (contador > 0)
            {
                _unitOfWork.Commit();
            }
        }

        public Pessoa Buscar(int id)
        {
            return _pessoaRepository.BuscarPorId(id);
        }

        public Pessoa Buscar(int? idEacesso)
        {
            return _pessoaRepository.Buscar(idEacesso);
        }



        private List<Pessoa> ObterPessoasEAcesso()
        {
            List<Pessoa> pessoas;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = "select nome as Nome, lower(isnull(email_int,'')) as email, id as ideacesso from stfcorp.tblprofissionais";
                pessoas = dbConnection.Query<Pessoa>(sQuery).AsList();
            }

            return pessoas;
        }

        public void AtualizarMigracao()
        {
            AtualizarEmailPessoasAntigas();
            List<Pessoa> pessoas = ObterPessoasNovasEAcesso();
            PersistirPessoas(pessoas);
        }

        private List<Pessoa> ObterPessoasNovasEAcesso()
        {
            var idsEacessoExistentes = _pessoaRepository.BuscarTodosIdsEacesso();
            List<Pessoa> pessoasEacesso = ObterPessoasEacesso();

            var pessoasNovas = pessoasEacesso.Where(x => !idsEacessoExistentes.Any(y => y == x.IdEacesso)).ToList();
            return pessoasNovas;
        }

        private void AtualizarEmailPessoasAntigas()
        {
            var pessoasStfcorp = _pessoaRepository.BuscarTodos();
            List<Pessoa> pessoasEacesso = ObterPessoasEacesso();
            List<Pessoa> pessoasParaAtualizar = new List<Pessoa>();
            
            foreach (var pessoa in pessoasStfcorp)
            {
                var pessoaEacesso = pessoasEacesso.FirstOrDefault(x => x.IdEacesso == pessoa.IdEacesso);
                if (pessoaEacesso != null)
                {
                    pessoa.EmailInterno = pessoaEacesso.Email;
                    pessoasParaAtualizar.Add(pessoa);
                }
                if (pessoasParaAtualizar.Count > 100)
                {
                    _pessoaRepository.AtualizarVariosSemAuditoria(pessoasParaAtualizar);
                    _unitOfWork.Commit();
                    pessoasParaAtualizar = new List<Pessoa>();
                }
            }
            if (pessoasParaAtualizar.Count > 0)
            {
                _pessoaRepository.AtualizarVariosSemAuditoria(pessoasParaAtualizar);
                _unitOfWork.Commit();
            }
        }

        private List<Pessoa> ObterPessoasEacesso()
        {
            List<Pessoa> pessoasEacesso;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = @"select nome as Nome, lower(isnull(email_int,'')) as email, id as ideacesso from stfcorp.tblprofissionais";
                pessoasEacesso = dbConnection.Query<Pessoa>(sQuery).AsList();
            }

            return pessoasEacesso;
        }
    }
}
