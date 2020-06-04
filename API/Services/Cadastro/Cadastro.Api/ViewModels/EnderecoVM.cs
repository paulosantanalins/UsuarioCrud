namespace Cadastro.Api.ViewModels
{
    public class EnderecoVM
    {
        public int? IdCidade { get; set; }
        public int? IdCliente { get; set; }
        public int? IdEstado { get; set; }
        public string SgAbrevLogradouro { get; set; }
        public string DescricaoAbrevLogradouro { get; set; }
        public string NmEndereco { get; set; }
        public string NrEndereco { get; set; }
        public string NmCompEndereco { get; set; }
        public string NmBairro { get; set; }
        public string NrCep { get; set; }
        public string DescricaoCidade { get; set; }
        public string DescricaoEstado { get; set; }
        public string DescricaoPais { get; set; }
    }
}
