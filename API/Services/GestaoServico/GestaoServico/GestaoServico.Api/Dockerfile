FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services/GestaoServico/GestaoServico/GestaoServico.Api/GestaoServico.Api.csproj Services/GestaoServico/GestaoServico/GestaoServico.Api/
COPY Services/GestaoServico/GestaoServico/GestaoServico.Domain/GestaoServico.Domain.csproj Services/GestaoServico/GestaoServico/GestaoServico.Domain/
COPY Services/GestaoServico/GestaoServico/GestaoServico.Domain.Core/GestaoServico.Domain.Core.csproj Services/GestaoServico/GestaoServico/GestaoServico.Domain.Core/
COPY Services/GestaoServico/GestaoServico/GestaoServico.Infra.Data.SqlServer/GestaoServico.Infra.Data.SqlServer.csproj Services/GestaoServico/GestaoServico/GestaoServico.Infra.Data.SqlServer/
COPY Services/GestaoServico/GestaoServico/GestaoServico.Infra.CrossCutting.IoC/GestaoServico.Infra.CrossCutting.IoC.csproj Services/GestaoServico/GestaoServico/GestaoServico.Infra.CrossCutting.IoC/
RUN dotnet restore Services/GestaoServico/GestaoServico/GestaoServico.Api/GestaoServico.Api.csproj
COPY . .
WORKDIR /src/Services/GestaoServico/GestaoServico/GestaoServico.Api
RUN dotnet build GestaoServico.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GestaoServico.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GestaoServico.Api.dll"]
