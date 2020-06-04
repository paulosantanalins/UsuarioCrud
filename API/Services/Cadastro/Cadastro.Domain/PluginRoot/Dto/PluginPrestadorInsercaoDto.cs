using System;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class PluginPrestadorInsercaoDto
    {

        public PluginPrestadorInsercaoDto()
        {
            StatusInt = "I";
            DataInsercao = DateTime.Now;
            Inativo = false;
        }

        public string StatusInt { get; set; }
        public string OperacaoInt { get; set; }
        public DateTime DataInsercao { get; set; }
        public int? SeqlEACesso { get; set; }
        public string CodColigada { get; set; }
        public string CodCfo { get; set; }
        public string Nome { get; set; }
        public string Rua { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CodEtd { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Fax { get; set; }
        public bool Inativo { get; set; }
        public string Email { get; set; }
        public string Pais { get; set; }

        public string IdCliente { get; set; }
        public string Cliente { get; set; }
        public int IdProfissional { get; set; }
    }
}
