using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces;
using RepasseEAcesso.Domain.SharedRoot;
using RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces;
using System;
using System.Collections.Generic;
using Utils.Extensions;

namespace RepasseEAcesso.Domain.RepasseRoot.Service
{
    public class RepasseService : IRepasseService
    {
        private readonly IRepasseRepository _repasseRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IClienteServicoRepository _clienteServicoRepository;
        private readonly IProfissionaisRepository _profissionaisRepository;
        private readonly IRepasseNivelUmRepository _repasseNivelUmRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPeriodoRepasseService _periodoRepasseService;

        public RepasseService(IRepasseRepository repasseRepository,
                              IClienteRepository clienteRepository,
                              IClienteServicoRepository clienteServicoRepository,
                              IProfissionaisRepository profissionaisRepository,
                               IRepasseNivelUmRepository repasseNivelUmRepository,
                               IPeriodoRepasseService periodoRepasseService,
            IUnitOfWork unitOfWork)
        {
            _repasseRepository = repasseRepository;
            _clienteRepository = clienteRepository;
            _clienteServicoRepository = clienteServicoRepository;
            _profissionaisRepository = profissionaisRepository;
            _repasseNivelUmRepository = repasseNivelUmRepository;
            _unitOfWork = unitOfWork;
            _periodoRepasseService = periodoRepasseService;
        }

        public Repasse BuscarPorId(int id)
        {
            var repasse = _repasseRepository.BuscarPorId(id);
            return repasse;
        }

        public void RealizarMigracaoRepasseEacesso()
        {

            var repasses = _repasseRepository.BuscarTodosMigracaoRepasse();
            var repasseSTFCORP = _repasseNivelUmRepository.BuscarTodos();

            List<RepasseNivelUm> repasseNivelUmLista = new List<RepasseNivelUm>();

            var periodoVigente = _periodoRepasseService.BuscarPeriodoVigente();

            var countAdd = 0;
            var countTotal = 0;
            repasses.ForEach(r =>
            {

                var clienteOrigiem = obterNomeCliente(r.IdClienteOrigem);
                var clienteDestino = _clienteRepository.BuscarPorId(r.IdClienteDestino).NomeFantasia;

                var servicoOrigiem = obterNomeServico(r.IdServicoOrigem);
                var servicoDestino = _clienteServicoRepository.BuscarPorId(r.IdServicoDestino).Nome;

                var nomeProficiional = obterNomeProfissional(r.IdProfissional);


                var repasseNivelUm = new RepasseNivelUm
                {
                    IdClienteOrigem = r.IdClienteOrigem.GetValueOrDefault(),
                    IdClienteDestino = r.IdClienteDestino,
                    IdCelulaDestino = r.IdCelulaDestino,
                    DataRepasse = r.DataRepasse,
                    DescricaoProjeto = r.DescricaoProjeto,
                    IdCelulaOrigem = r.IdCelulaOrigem,
                    IdEpm = 0,//Não mapeado
                    IdMoeda = r.IdMoeda,
                    IdOrigem = r.IdOrigem,
                    IdProfissional = r.IdProfissional,
                    IdRepasseEacesso = r.Id,
                    IdRepasseMaeEAcesso = r.IdRepasseMae,
                    IdServicoDestino = r.IdServicoDestino,
                    IdServicoOrigem = r.IdServicoOrigem.GetValueOrDefault(),
                    Justificativa = r.Justificativa,
                    MotivoNegacao = r.MotivoNegacao,
                    NomeClienteDestino = clienteDestino,
                    NomeClienteOrigem = clienteOrigiem,
                    NomeProfissional = nomeProficiional,
                    NomeServicoDestino = servicoDestino,
                    NomeServicoOrigem = servicoOrigiem,
                    QuantidadeItens = (int)r.QuantidadeItens,
                    RepasseInterno = false, //não mapeado
                    Status = r.Status,
                    ValorCustoProfissional = r.ValorCustoProfissional,
                    ValorTotal = r.ValorTotal,
                    ValorUnitario = r.ValorUnitario,                    
                    DataAlteracao = DateTime.Now,
                    Usuario = "STFCORP"                    
                };


                //TODO IMPLEMENATAR e TESTAR 
                LogRepasse log = new LogRepasse
                {
                    IdStatusRepasse = 4849,
                    Descricao = SharedEnuns.StatusRepasseEacesso.NAO_ANALISADO.GetDescription(),
                    DataAlteracao = repasseNivelUm.DataAlteracao,
                    Usuario = repasseNivelUm.Usuario

                };

                repasseNivelUm.LogsRepasse.Add(log);

                repasseNivelUmLista.Add(repasseNivelUm);               

                countAdd++;
                countTotal++;
                bool encontrado = false;

                if (countAdd == 100 || repasses.Count == countAdd)
                {
                    _repasseNivelUmRepository.AdicionarRange(repasseNivelUmLista);

                    foreach(var repasse in repasseSTFCORP)
                    {
                        if (repasseNivelUm.IdRepasseEacesso == repasse.IdRepasseEacesso)
                            encontrado = true;
                    }

                    if (!encontrado)
                        _unitOfWork.Commit();
                    else
                        encontrado = false;

                    countAdd = 0;
                    repasseNivelUmLista = new List<RepasseNivelUm>();
                };
            });           
        }

        private string obterNomeCliente(int? idCliente)
        {
            if (idCliente != null || idCliente > 0) {
                var cliente = _clienteRepository.BuscarPorId((int) idCliente);
              return cliente == null ? string.Empty : cliente.NomeFantasia;
            }
            return string.Empty;
        }

        private string obterNomeServico(int? idServico)
        {
            if (idServico != null || idServico > 0)
            {
                var servico = _clienteServicoRepository.BuscarPorId((int) idServico);
                return servico == null ? string.Empty : servico.Nome;
            }
            return string.Empty;
        }

        private string obterNomeProfissional(int? idProfissional)
        {
            if (idProfissional != null || idProfissional > 0)
            {
                var proffisional = _profissionaisRepository.BuscarPorId((int)idProfissional);
                return proffisional == null ? string.Empty : proffisional.Nome;
            }
            return string.Empty;
        }

        public void RealizarAtualizaçãoIdRepasseMaeEacesso()
        {

            var repasseNivelUmLista = _repasseNivelUmRepository.BuscarComInclude();
         

            var countAdd = 0;
            var countTotal = 0;
            repasseNivelUmLista.ForEach(r =>
            {
                var repasseNivelUmListaLegado = _repasseNivelUmRepository.BuscarTodosFilhosLegado((int)r.IdRepasseEacesso);


                repasseNivelUmListaLegado.ForEach(x =>
               {
                   x.IdRepasseMae = r.Id;
               });

                _repasseNivelUmRepository.AdicionarRange(repasseNivelUmListaLegado);

                countAdd++;
                countTotal++;
                if (countAdd == 100 || repasseNivelUmLista.Count == countAdd)
                {   
                    _unitOfWork.Commit();
                    countAdd = 0;
                    repasseNivelUmLista = new List<RepasseNivelUm>();
                };
            });
        }

    }
}
