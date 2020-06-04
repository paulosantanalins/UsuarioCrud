using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Domain.ClienteRoot.Service.Interfaces;
using Cliente.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Salesforce.Models;

namespace Cliente.Domain.ClienteRoot.Service
{
    public class EnderecoService : IEnderecoService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IEnderecoRepository _enderecoRepository;
        protected readonly IPaisRepository _paisRepository;
        protected readonly ICidadeRepository _cidadeRepository;
        protected readonly IEstadoRepository _estadoRepository;
        public EnderecoService(IUnitOfWork unitOfWork,
                               IEnderecoRepository enderecoRepository,
                               IPaisRepository paisRepository,
                               ICidadeRepository cidadeRepository,
                               IEstadoRepository estadoRepository)
        {
            _unitOfWork = unitOfWork;
            _enderecoRepository = enderecoRepository;
            _paisRepository = paisRepository;
            _cidadeRepository = cidadeRepository;
            _estadoRepository = estadoRepository;
        }


        public Endereco TratarEnderecoSalesForce(AccountSalesObject salesObject, Endereco endereco)
        {
            var cidadeDb = _cidadeRepository.BuscarUnicoPorParametro(x => x.NmCidade.ToUpper() == salesObject.CNPJ_Cidade__c.ToUpper());
            if (cidadeDb == null)
            {
                cidadeDb = new Cidade
                {
                    NmCidade = salesObject.CNPJ_Estado__c,
                };
                endereco.Cidade = cidadeDb;
                var estadoDb = _estadoRepository.BuscarUnicoPorParametro(x => x.SgEstado.ToUpper() == salesObject.CNPJ_Estado__c.ToUpper() || x.NmEstado.ToUpper() == salesObject.CNPJ_Estado__c.ToUpper());
                if (estadoDb == null)
                {
                    estadoDb = new Estado
                    {
                        NmEstado = salesObject.CNPJ_Estado__c,
                        SgEstado = salesObject.CNPJ_Estado__c.Substring(0, 1),
                    };
                    endereco.Cidade.Estado = estadoDb;
                    var paisDb = _paisRepository.BuscarUnicoPorParametro(x => x.SgPais.ToUpper() == salesObject.PaisConta__c.ToUpper() || x.NmPais.ToUpper() == salesObject.PaisConta__c.ToUpper());
                    if (paisDb == null)
                    {
                        paisDb = new Pais
                        {
                            NmPais = salesObject.PaisConta__c,
                            SgPais = salesObject.PaisConta__c.Substring(0, 1),
                        };
                        endereco.Cidade.Estado.Pais = paisDb;
                    }
                    else
                    {
                        endereco.Cidade.Estado.IdPais = paisDb.Id;
                    }
                }
                else
                {
                    endereco.Cidade.IdEstado = estadoDb.Id;
                }
            }
            else
            {
                endereco.IdCidade = cidadeDb.Id;
            }
            return endereco;
        }

        public int? VerificarEndereco(string cidade)
        {
            if (cidade != null)
            {
                var cidadeDb = _cidadeRepository.BuscarUnicoPorParametro(x => x.NmCidade.ToUpper() == cidade.ToUpper());
                if (cidadeDb != null)
                {
                    return cidadeDb.Id;
                }
            }
            return null;
        }
    }
}
