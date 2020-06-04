FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Services/EnvioEmail/EnvioEmail.Api/EnvioEmail.Api.csproj Services/EnvioEmail/EnvioEmail.Api/
COPY Services/EnvioEmail/EnvioEmail.Infra.Data.SqlServer/EnvioEmail.Infra.Data.SqlServer.csproj Services/EnvioEmail/EnvioEmail.Infra.Data.SqlServer/
COPY Services/EnvioEmail/EnvioEmail.Domain/EnvioEmail.Domain.csproj Services/EnvioEmail/EnvioEmail.Domain/
COPY Services/EnvioEmail/EnvioEmail.Domain.Core/EnvioEmail.Domain.Core.csproj Services/EnvioEmail/EnvioEmail.Domain.Core/
COPY Services/EnvioEmail/EnvioEmail.Infra.CrossCutting.IoC/EnvioEmail.Infra.CrossCutting.IoC.csproj Services/EnvioEmail/EnvioEmail.Infra.CrossCutting.IoC/
RUN dotnet restore
COPY . .
WORKDIR /src/Services/EnvioEmail/EnvioEmail.Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EnvioEmail.Api.dll"]
