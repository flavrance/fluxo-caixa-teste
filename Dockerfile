FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY src/FluxoCaixa.API/*.csproj ./src/FluxoCaixa.API/
COPY src/FluxoCaixa.Application.Core/*.csproj ./src/FluxoCaixa.Application.Core/
COPY src/FluxoCaixa.Domain.Core/*.csproj ./src/FluxoCaixa.Domain.Core/
COPY src/FluxoCaixa.Infrastructure.Data/*.csproj ./src/FluxoCaixa.Infrastructure.Data/
COPY src/FluxoCaixa.Infrastructure.IoC/*.csproj ./src/FluxoCaixa.Infrastructure.IoC/
COPY src/FluxoCaixa.Worker/*.csproj ./src/FluxoCaixa.Worker/
COPY src/FluxoCaixa.Tests/*.csproj ./src/FluxoCaixa.Tests/

RUN dotnet restore FluxoCaixa.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out src/FluxoCaixa.API/FluxoCaixa.API.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENTRYPOINT ["dotnet", "FluxoCaixa.API.dll"]