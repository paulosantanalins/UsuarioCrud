FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services/Account/Account.Api/Account.Api.csproj Services/Account/Account.Api/
COPY Services/Account/Account.Infra.CrossCutting.IoC/Account.Infra.CrossCutting.IoC.csproj Services/Account/Account.Infra.CrossCutting.IoC/
COPY Services/Account/Account.Domain.Core/Account.Domain.Core.csproj Services/Account/Account.Domain.Core/
COPY Services/Account/Account.Domain/Account.Domain.csproj Services/Account/Account.Domain/
COPY Services/Account/Account.Infra.Data.SqlServer/Account.Infra.Data.SqlServer.csproj Services/Account/Account.Infra.Data.SqlServer/
RUN dotnet restore Services/Account/Account.Api/Account.Api.csproj
COPY . .
WORKDIR /src/Services/Account/Account.Api
RUN dotnet build Account.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Account.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Account.Api.dll"]
