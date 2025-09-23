# Dockerfile para Railway
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto
COPY *.sln .
COPY Fulbito.Api/*.csproj ./Fulbito.Api/
COPY Fulbito.Core/*.csproj ./Fulbito.Core/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY . .

# Publicar la aplicación
RUN dotnet publish Fulbito.Api/Fulbito.Api.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Railway asigna el puerto automáticamente
EXPOSE $PORT

ENTRYPOINT ["dotnet", "Fulbito.Api.dll"]