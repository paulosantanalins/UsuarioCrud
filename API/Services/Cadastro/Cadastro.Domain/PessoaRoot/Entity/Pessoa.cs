using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PessoaRoot.Entity
{
    public class Pessoa : EntityBase
    {
        public string Matricula { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string NomeDoPai { get; set; }
        public string NomeDaMae { get; set; }
        public string Email { get; set; }
        public string EmailInterno { get; set; }
        public int? CodEacessoLegado { get; set; }
        public Prestador Prestador { get; set; }
        public int? IdNacionalidade { get; set; }
        public virtual DomNacionalidade Nacionalidade { get; set; }
        public int? IdEscolaridade { get; set; }
        public virtual DomEscolaridade Escolaridade { get; set; }
        public int? IdExtensao { get; set; }
        public virtual DomExtensao Extensao { get; set; }
        public int? IdGraduacao { get; set; }
        public virtual DomGraduacao Graduacao { get; set; }
        public int? IdEstadoCivil { get; set; }
        public virtual DomEstadoCivil EstadoCivil { get; set; }
        public int? IdSexo { get; set; }
        public virtual DomSexo Sexo { get; set; }
        public int? IdEndereco { get; set; }
        public virtual Endereco Endereco { get; set; }
        public int? IdTelefone { get; set; }
        public virtual Telefone Telefone { get; set; }
        public virtual ICollection<Celula> Celulas { get; set; }
    }
}
