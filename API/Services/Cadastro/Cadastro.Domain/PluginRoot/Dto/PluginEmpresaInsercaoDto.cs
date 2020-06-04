using System;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class PluginEmpresaInsercaoDto
    {
        public PluginEmpresaInsercaoDto()
        {
            CodColigada = 0;
            Ativo = true;
            PagRec = 3;
            TipoBairro = 1;
            StatusInt = "I";
            PessoaFisOuJur = "J";
            CodColTcf = 0;
            CodTcf = 15;
            DataCriacao = DateTime.Now;
            CAplicativoOrigem = "tblProfissionaisEmpresas";
            Funcao = "Profissional PJ";
            CodAplic = "L";
            ChaveOrigemInt = null;
        }

        public int CodColigada { get; set; }
        public bool Ativo { get; set; }
        public int IdPais { get; set; }
        public int PagRec { get; set; }
        public int TipoBairro { get; set; }
        public int TipoRua { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? SeqlEACesso { get; set; }
        public string CodCfo { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string CgCcfo { get; set; }
        public string CiUf { get; set; }
        public string Pais { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Cidade { get; set; }
        public string Rua { get; set; }
        public string CodMunicipio { get; set; }
        public string InscrEstadual { get; set; }
        public string Nome { get; set; }
        public string StatusInt { get; set; }
        public string OperacaoInt { get; set; }
        public string PessoaFisOuJur { get; set; }
        public int CodColTcf { get; set; }
        public int CodTcf { get; set; }
        public int OptantePeloSimples { get; set; }

        public int? ChaveOrigemInt { get; set; }
        public string CAplicativoOrigem { get; set; }

        public string NomeCont { get; set; }
        public string RuaCont { get; set; }
        public string NumeroCont { get; set; }
        public string ComplementoCont { get; set; }
        public string BairroCont { get; set; }
        public string CidadeCont { get; set; }
        public string CodEtd { get; set; }
        public string CodEtdCont { get; set; }
        public string CepCont { get; set; }
        public string Funcao { get; set; }
        public string TelefoneCont { get; set; }
        public string CelularCont { get; set; }
        public string EmailCont { get; set; }
        public string PaisCont { get; set; }
        public int IdContato { get; set; }
        public string RamalCont { get; set; }
        public DateTime DataNascimentoCont { get; set; }
        public string IdCidadeCont { get; set; }
        public string CodAplic { get; set; }
    }
}
