using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ControleAcesso.Domain.SharedRoot
{
    public class SharedEnuns
    {
        public enum DominioTipoHierarquia
        {
            [Description("TIPO_HIERARQUIA")]
            VL_TIPO_DOMINIO,            
        }

        public enum DominioTipoContabil
        {
            [Description("TIPO_CONTABIL")]
            VL_TIPO_DOMINIO,            
        }

        public enum DominioTipoServicoDelivery
        {
            [Description("TIPO_SERVICO_DELIVERY")]
            VL_TIPO_DOMINIO,            
        }

        public enum NomeCamposCadastroCelula
        {
            [Description("NÚMERO")]
            NUMERO,
            [Description("DESCRIÇÃO")]
            DESCRICAO,
            [Description("PAÍS")]
            PAIS,
            [Description("MOEDA")]
            MOEDA,
            [Description("CÉLULA RESPONSÁVEL")]
            CELULA_RESPONSAVEL,
            [Description("RESPONSÁVEL")]
            PESSOA_RESPONSAVEL,
            [Description("HIERARQUIA CÉLULA")]
            TIPO_HIERARQUIA,
            [Description("TIPO CÉLULA")]
            TIPO_CELULA,
            [Description("TIPO CONTÁBIL")]
            TIPO_CONTABIL,
            [Description("GRUPO")]
            GRUPO,
            [Description("CÉLULA SUPERIOR")]
            CELULA_SUPERIOR,
            [Description("TIPO SERVICO DELIVERY")]
            TIPO_SERVICO_DELIVERY,
            [Description("EMPRESA DO GRUPO")]
            EMPRESA_GRUPO,
            [Description("REPASSE CÉLULA")]
            REPASSE_CELULA,
            [Description("REPASSE EPM")]
            REPASSE_EPM,
            [Description("STATUS")]
            STATUS
        }

        public enum StatusCelula
        {
            [Description("Ativo")]
            Ativada = 0,
            [Description("Inativado")]
            Inativada = 1,
            [Description("Célula Processo de Inativação")]
            PendenteParaInativacao = 2,
            [Description("Reativado")]
            Reativada = 3
        }
    }
}
