using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class Prestador : EntityBase
    {
        public Prestador()
        {
            Pessoa = new Pessoa();
            HorasMesPrestador = new HashSet<HorasMesPrestador>();
            ValoresPrestador = new HashSet<ValorPrestador>();
            ClientesServicosPrestador = new HashSet<ClienteServicoPrestador>();
        }

        public int? IdEmpresaGrupo { get; set; }
        public int IdCelula { get; set; }
        public Celula Celula { get; set; }
        public int IdFilial { get; set; }
        public string ProdutoRm { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataValidadeContrato { get; set; }
        public DateTime? DataDesligamento { get; set; }
        public DateTime? DataUltimaAlteracaoEacesso { get; set; }
        public DateTime? DataInicioCLT { get; set; }
        public bool PagarPelaMatriz { get; set; }
        public int IdProdutoRm { get; set; }
        public string IdRepresentanteRmTRPR { get; set; }
        public string EmailParceiro { get; set; }
        public int? CodEacessoLegado { get; set; }
        public int IdCargo { get; set; }
        public int IdPessoa { get; set; }
        public int? IdPessoaAlteracao { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual DomCargo Cargo { get; set; }
        public int IdContratacao { get; set; }
        public virtual DomContratacao Contratacao { get; set; }
        public int? IdSituacao { get; set; }
        public virtual DomSituacaoPrestador SituacaoPrestador { get; set; }
        public int? IdDiaPagamento { get; set; }
        public virtual DomDiaPagamento DiaPagamento { get; set; }
        public int? IdAreaFormacao { get; set; }
        public virtual DomAreaFormacao AreaFormacao { get; set; }
        public int? IdTipoRemuneracao { get; set; }
        public virtual DomTipoRemuneracao TipoRemuneracao { get; set; }
        public virtual ICollection<HorasMesPrestador> HorasMesPrestador { get; set; }
        public virtual ICollection<ValorPrestador> ValoresPrestador { get; set; }
        public virtual ICollection<EmpresaPrestador> EmpresasPrestador { get; set; }
        public virtual ICollection<InativacaoPrestador> InativacoesPrestador { get; set; }
        public virtual ICollection<ClienteServicoPrestador> ClientesServicosPrestador { get; set; }
        public virtual ICollection<ObservacaoPrestador> ObservacoesPrestador { get; set; }
        public virtual ICollection<ContratoPrestador> ContratosPrestador { get; set; }
        public virtual ICollection<DocumentoPrestador> DocumentosPrestador { get; set; }
        public virtual ICollection<TransferenciaPrestador> TransferenciasPrestador { get; set; }
        public virtual ICollection<TransferenciaCltPj> TransferenciasCltPj { get; set; }
        public virtual ICollection<FinalizacaoContrato> FinalizacoesContratos { get; set; }
        public virtual ICollection<ReajusteContrato> ReajustesContratos { get; set; }
    }
}
