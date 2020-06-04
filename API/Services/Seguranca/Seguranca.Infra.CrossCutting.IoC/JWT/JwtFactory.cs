using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;

namespace Seguranca.Infra.CrossCutting.IoC.JWT
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new List<Claim>()
            {
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.NomeCompleto),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Email),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Password),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Celula),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.CelulaTratada),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.UidNumber),
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.CelulasVisualizadas)
             };

            //foreach (var claim in identity.Claims)
            //{
            //    claims.Add(new Claim(claim.Type, claim.Value));
            //}
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims.ToArray(),
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public string GenerateEncodedTokenLogin(string userName, ClaimsIdentity identity)
        {
            var claims = new List<Claim>()
            {
                 identity.FindFirst(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login)
             };

            //foreach (var claim in identity.Claims)
            //{
            //    claims.Add(new Claim(claim.Type, claim.Value));
            //}
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims.ToArray(),
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, string id)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.NomeCompleto, id),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Rol, JwtUtils.Claims.Strings.JwtClaims.ApiAccess)
            });
        }

        public ClaimsIdentity GenerateClaimsIdentity(string login)
        {
            return new ClaimsIdentity(new GenericIdentity(login, "Token"), new[]
            {
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login, login),
            });
        }

        public ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email)
        {
            return new ClaimsIdentity(new GenericIdentity(login, "Token"), new[]
            {
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.NomeCompleto, nomeCompleto),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login, login),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Email, email)
            });
        }

        public ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email, string password)
        {
            return new ClaimsIdentity(new GenericIdentity(login, "Token"), new[]
            {
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.NomeCompleto, nomeCompleto),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login, login),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Email, email),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Password, "")
                //new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Password, password)
            });
        }

        public ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email, string password, string celula)
        {
            return new ClaimsIdentity(new GenericIdentity(login, "Token"), new[]
            {
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.NomeCompleto, nomeCompleto),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login, login),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Email, email),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Password, ""),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Celula, celula)
                //new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Password, password)
            });
        }

        public ClaimsIdentity GenerateClaimsIdentity(string login, string nomeCompleto, string email, string password, string celula, string uidNumber, List<int> celulasVisualizadas, string celulaTratada)
        {
            return new ClaimsIdentity(new GenericIdentity(login, "Token"), new[]
            {
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.NomeCompleto, nomeCompleto),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Login, login),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Email, email),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Password, ""),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.Celula, celula),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.CelulaTratada, celulaTratada),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.UidNumber, uidNumber),
                new Claim(JwtUtils.Claims.Strings.JwtClaimIdentifiers.CelulasVisualizadas, string.Join(",", celulasVisualizadas))
            });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
