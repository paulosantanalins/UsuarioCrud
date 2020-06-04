using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Seguranca.Infra.CrossCutting.IoC.JWT
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id);
        ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email);
        ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email, string password);
        ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email, string password,string celula);
        ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email, string password, string celula, string uidNumber, List<int> celulasVisualizadas, string celulaTratada);
        ClaimsIdentity GenerateClaimsIdentity(string login);
        string GenerateEncodedTokenLogin(string userName, ClaimsIdentity identity);
    }
}
