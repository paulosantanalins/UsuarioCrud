FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services/ControleAcesso/ControleAcesso.Api/ControleAcesso.Api.csproj Services/ControleAcesso/ControleAcesso.Api/
COPY Services/ControleAcesso/ControleAcesso.Domain/ControleAcesso.Domain.csproj Services/ControleAcesso/ControleAcesso.Domain/
COPY Services/ControleAcesso/ControleAcesso.Domain.Core/ControleAcesso.Domain.Core.csproj Services/ControleAcesso/ControleAcesso.Domain.Core/
COPY Services/ControleAcesso/ControleAcesso.Infra.CrossCutting.IoC/ControleAcesso.Infra.CrossCutting.IoC.csproj Services/ControleAcesso/ControleAcesso.Infra.CrossCutting.IoC/
COPY Services/ControleAcesso/ControleAcesso.Infra.Data.SqlServer/ControleAcesso.Infra.Data.SqlServer.csproj Services/ControleAcesso/ControleAcesso.Infra.Data.SqlServer/
RUN dotnet restore Services/ControleAcesso/ControleAcesso.Api/ControleAcesso.Api.csproj
COPY . .
WORKDIR /src/Services/ControleAcesso/ControleAcesso.Api
RUN dotnet build ControleAcesso.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ControleAcesso.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ControleAcesso.Api.dll"]
