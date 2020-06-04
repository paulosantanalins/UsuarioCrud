using Newtonsoft.Json;
using Seguranca.Infra.CrossCutting.IoC.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Seguranca.Infra.CrossCutting.IoC.JwtUtils
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                login = identity.Claims.Single(c => c.Type == Claims.Strings.JwtClaimIdentifiers.Login).Value,
                name = identity.Claims.Single(c => c.Type == Claims.Strings.JwtClaimIdentifiers.NomeCompleto).Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }

        public static async Task<string> GenerateJwtLogin(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            return await Task.Run(() => jwtFactory.GenerateEncodedTokenLogin(userName, identity));
        }
    }
}
