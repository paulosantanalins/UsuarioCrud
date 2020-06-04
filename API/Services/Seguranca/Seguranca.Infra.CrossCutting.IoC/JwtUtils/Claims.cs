using System;
using System.Collections.Generic;
using System.Text;

namespace Seguranca.Infra.CrossCutting.IoC.JwtUtils
{
    public static class Claims
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id", Codigo = "codigo", Name = "name", Login = "login", Email = "email", NomeCompleto = "nomeCompleto", Password = "password", Celula = "celula", UidNumber="uidNumber", CelulasVisualizadas = "celulasVisualizadas", CelulaTratada = "celulaTratada";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access", valorCodigo = "1234";
            }
        }
    }
}
