using Cadastro.Domain.NatcorpRoot.Dto;
using Cadastro.Domain.NatcorpRoot.Repository;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Utils.Connections;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
	public class ProfissionalNatcorpRepository : IProfissionalNatcorpRepository
    {
        private readonly IOptions<ConnectionStrings> _options;

        public ProfissionalNatcorpRepository(IOptions<ConnectionStrings> options)
        {
            _options = options;
        }

        public IEnumerable<ProfissionalNatcorpDto> BuscarProfissionaisNatcorpParaMigracao(DateTime? data = null)
        {
            using (var connection = new SqlConnection(_options.Value.EacessoConnection))
            {
                connection.Open();

				var query = $@"SELECT 
								 CodColigada
								,MATRICULA
								,CONVERT(varchar,DataAdmissao, 103) AS DataAdmissao
								,CONVERT(varchar,DtUltimaAtualizacao, 103) AS DtUltimaAtualizacao
								,UPPER(Nome) AS Nome
								,CPF
								,RG
								,CodSituacao	
								,F.IdFilial
								,CodCelula
								,CodCliente
								,CodServico
								,CodTipoContrato
								,CodNacionalidade
								,DtNascimento
								,Cod_Sexo
								,CodTipoCusto
								,IdCargo
								,CASE UPPER(NecessidadesEspeciais) WHEN 'SIM' THEN 1 ELSE 0 END
								,EnderecoCodCidade
								,EnderecoLogradouro
								,EnderecoNumero
								,EnderecoComplemento
								,EnderecoBairro
								,EnderecoCEP
								,CodEstadoCivil
								,UPPER(NomePai) AS NomePai
								,UPPER(NomeMae) AS NomeMae
								,UPPER(NomeConjuge)  AS NomeConjuge 
								,TelefoneResidencial
								,TelefoneResidencialDDD
								,TelefoneResidencialDDI
								,TelefoneCelular
								,TelefoneCelularDDD
								,TelefoneCelularDDI
								,CodBanco
								,CodAgencia
								,CodContaCorrente
								,CodMatricula
								,Matricula_RM
								,EG.IdEmpresa
								,EmailPessoal
								,PIS
								,CODSINDICATO
								,NOMESINDICATO
								FROM INTEGRACAO_Profissional_VW 
								INNER JOIN tblEmpresasGrupo eg WITH (NOLOCK) ON EG.ERPExterno = INTEGRACAO_Profissional_VW.CodColigada
								INNER JOIN tblFiliais F ON F.ERPExterno = INTEGRACAO_Profissional_VW.CodFilial AND F.IdEmpresa = EG.IdEmpresa
								WHERE  CodCelula > 0	
								AND CODSITUACAO NOT LIKE '9_'--< 90 -- ACIMA DE 90 SÃO OS DESLIGADOS DO NATCORP
								AND CodTipoContrato IN('CLT', 'EST', 'APZ', 'DIR')
								{(data != null ? "AND DtUltimaAtualizacao >= @Data" : "")};";

				return connection.Query<ProfissionalNatcorpDto>(query, new { Data = data });
            }

        }
    }
}
