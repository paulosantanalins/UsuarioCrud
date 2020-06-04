﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.ViewModels
{
    public class ClienteVM
    {
        public int IdCliente { get; set; }
        public string Cnpj { get; set; }
        public string NmFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string Tipo { get; set; }
        public string Endereco { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string TipoClienteRM { get; set; }
        public string SetorEconomico { get; set; }
        public string RamoAtividade { get; set; }
        public string Classificacao { get; set; }
        public string Telefone { get; set; }
        public List<ContatoClienteEacessoVM> Contatos { get; set; } = new List<ContatoClienteEacessoVM>();
        public List<ClienteLocalTrabalhoEacessoVM> LocaisTrabalho { get; set; } = new List<ClienteLocalTrabalhoEacessoVM>();   
        public string FlStatus { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
