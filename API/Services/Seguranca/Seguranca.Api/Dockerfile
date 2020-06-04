FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services/Seguranca/Seguranca.Api/Seguranca.Api.csproj Services/Seguranca/Seguranca.Api/
COPY Services/Seguranca/Seguranca.Domain.Core/Seguranca.Domain.Core.csproj Services/Seguranca/Seguranca.Domain.Core/
COPY Services/Seguranca/Seguranca.Infra.CrossCutting.IoC/Seguranca.Infra.CrossCutting.IoC.csproj Services/Seguranca/Seguranca.Infra.CrossCutting.IoC/
COPY Services/Seguranca/Seguranca.Infra.Data.SqlServer/Seguranca.Infra.Data.SqlServer.csproj Services/Seguranca/Seguranca.Infra.Data.SqlServer/
COPY Services/Seguranca/Seguranca.Domain/Seguranca.Domain.csproj Services/Seguranca/Seguranca.Domain/
RUN dotnet restore Services/Seguranca/Seguranca.Api/Seguranca.Api.csproj
COPY . .
WORKDIR /src/Services/Seguranca/Seguranca.Api
RUN dotnet build Seguranca.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Seguranca.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Seguranca.Api.dll"]
