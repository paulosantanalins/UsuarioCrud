using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Salesforce.Models;

namespace Cliente.Domain.ClienteRoot.Service.Interfaces
{
    public interface IEnderecoService
    {
        Endereco TratarEnderecoSalesForce(AccountSalesObject salesObject, Endereco endereco);
        int? VerificarEndereco(string cidade);
    }
}
