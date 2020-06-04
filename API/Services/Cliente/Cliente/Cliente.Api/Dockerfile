FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services/Cliente/Cliente/Cliente.Api/Cliente.Api.csproj Services/Cliente/Cliente/Cliente.Api/
COPY Services/Cliente/Cliente/Cliente.Infra.Data.SqlServer/Cliente.Infra.Data.SqlServer.csproj Services/Cliente/Cliente/Cliente.Infra.Data.SqlServer/
COPY Services/Cliente/Cliente/Cliente.Domain/Cliente.Domain.csproj Services/Cliente/Cliente/Cliente.Domain/
COPY Services/Cliente/Cliente/Cliente.Domain.Core/Cliente.Domain.Core.csproj Services/Cliente/Cliente/Cliente.Domain.Core/
COPY Services/Cliente/Cliente/Cliente.Infra.CrossCutting.IoC/Cliente.Infra.CrossCutting.IoC.csproj Services/Cliente/Cliente/Cliente.Infra.CrossCutting.IoC/
RUN dotnet restore Services/Cliente/Cliente/Cliente.Api/Cliente.Api.csproj
COPY . .
WORKDIR /src/Services/Cliente/Cliente/Cliente.Api
RUN dotnet build Cliente.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Cliente.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cliente.Api.dll"]
