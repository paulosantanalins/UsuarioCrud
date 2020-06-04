using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels
{
    public class ServicoIntegracaoVM
    {
        public ServicoIntegracaoVM()
        {

        }

        public ServicoIntegracaoVM(
            int    idCelula,
            string CNPJ,
            string nome,
            string descricao,
            string escopo,
            string contrato,
            string categoria,
            string siglaTipoServico,
            string dtFinalizacao,
           decimal markup,
           decimal valorKM,
            string flagReembolso,
            string flagQuarter,
            string delivery,
            int?   idCelulaDelivery,
            int?   idDelivery,
            string extrasReemb,
           decimal rentabilidade,
            string idSalesForce,
            string mensal,
            string codificacao,
            string localServico,
            string cnpjFilial,
           decimal? vHoraExtra,
            string horasExtrasPgMensal,
            string usuario,
            string login,
            string senha)
        {
            IdCelula = idCelula;
            this.CNPJ = CNPJ;
            Nome = nome != null ? nome.ToUpper() : "SALESFORCE#";
            Descricao = descricao != null || descricao != "" ? descricao.ToUpper() : Nome;
            Escopo = escopo != null ? escopo.ToUpper() : escopo;
            Contrato = contrato != null ? contrato.ToUpper() : contrato;
            SiglaTipoServico = siglaTipoServico != null ? siglaTipoServico.ToUpper() : "LOCS";
            if (dtFinalizacao != null)
            {
                DtFinalizacao = DateTime.Parse(dtFinalizacao, new CultureInfo("pt-BR"));
            }
            else
            {
                DtFinalizacao = null;
            }

            Markup = markup;
            //try { tDados.markup = markup; }
            //catch { erro = "markup"; tDados.markup = 21; }

            ValorKM = valorKM;
            //try { tDados.valorKM = valorKM; }
            //catch { erro = "valorKM"; tDados.valorKM = 0; }

            FlagReembolso = flagReembolso == "1" ? true : false;
            //try { tDados.flagReembolso = flagReembolso; }
            //catch { erro = "flagReembolso"; tDados.flagReembolso = "0"; }
            FlagQuarter = flagQuarter == "1" ? true : false;
            //try { tDados.flagQuarter = flagQuarter; }
            //catch { erro = "flagQuarter"; tDados.flagQuarter = "0"; }
            IdCelulaDelivery = idCelulaDelivery;
            //try { tDados.idCelulaDelivery = idCelulaDelivery; }
            //catch { erro = "idCelulaDelivery;"; tDados.idCelulaDelivery = 0; }
            IdDelivery = idDelivery;
            //try { tDados.idDelivery = idDelivery; }
            //catch { erro = "idDelivery"; tDados.idDelivery = 0; }
            Delivery = delivery;
            //try { tDados.delivery = delivery; }
            //catch { erro = "Delivery"; tDados.delivery = ""; }
            Rentabilidade = rentabilidade;
            //try { tDados.rentabilidade = rentabilidade; }
            //catch { erro = "rentabilidade"; tDados.rentabilidade = 0; }
            if (Int32.TryParse(extrasReemb, out int n))
            {
                ExtrasReemb = Int32.Parse(extrasReemb);
            }
            else
            {
               ExtrasReemb = null;
            }
            //try { tDados.extrasReemb = extrasReemb; }
            //catch { erro = "extrasReemb"; tDados.extrasReemb = "0"; }

            IdSalesForce = idSalesForce != null ? idSalesForce : "";
            //try { tDados.idSalesForce = idSalesForce; }
            //catch { erro = "idSalesForce"; tDados.idSalesForce = ""; }

            Mensal = mensal != null && mensal == "S" ? true : false;
            //try { tDados.mensal = mensal; }
            //catch { erro = "mensal"; tDados.mensal = "N"; }
            Codificacao = codificacao;
            //try { tDados.codificacao = codificacao; }
            //catch { erro = "codificacao"; tDados.codificacao = ""; }
            LocalServico = localServico;
            //try { tDados.localServico = localServico; }
            //catch { erro = "localServico"; tDados.localServico = ""; }
            CnpjFilial = cnpjFilial;
            //try { tDados.cnpjFilial = cnpjFilial; }
            //catch { erro = "cnpjFilial"; tDados.cnpjFilial = ""; }
            VHoraExtra = vHoraExtra;
            //try { tDados.ValorHoraExtra = vHoraExtra; }
            //catch { erro = "vHoraExtra"; tDados.ValorHoraExtra = -1; }
            HorasExtrasPgMensal = horasExtrasPgMensal;
            //try { tDados.horasExtrasPgMensal = horasExtrasPgMensal; }
            //catch { erro = "horasExtrasPgMensal"; tDados.horasExtrasPgMensal = ""; }
            IdSalesForce = idSalesForce;
            //try { tDados.usuario = idSalesForce; }
            //catch { erro = "usuario"; tDados.usuario = ""; }
            Usuario = usuario;
            Login = login;
            //try { tDados.login = login; }
            //catch { erro = "login"; tDados.login = ""; }
            Senha = senha;
        }

        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Escopo { get; set; }
        public string Contrato { get; set; }
        public string Categoria { get; set; }
        public string SiglaTipoServico { get; set; }
        public string StFinalizacao { get; set; }
        public decimal Markup { get; set; }
        public decimal ValorKM { get; set; }
        public bool FlagReembolso { get; set; }
        public bool FlagQuarter { get; set; }
        public DateTime? DtFinalizacao { get; set; }
        public string Delivery { get; set; }
        public int? IdCelulaDelivery { get; set; }
        public int? IdDelivery { get; set; }
        public int? ExtrasReemb { get; set; }
        public decimal? Rentabilidade { get; set; }
        public string IdSalesForce { get; set; }
        public bool Mensal { get; set; }
        public string Codificacao { get; set; }
        public string LocalServico { get; set; }
        public string CnpjFilial { get; set; }
        public decimal? VHoraExtra { get; set; }
        public string HorasExtrasPgMensal { get; set; }
        public string Usuario { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}
