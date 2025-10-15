# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el contenido del proyecto
COPY . .

# Restauramos dependencias
RUN dotnet restore "./Implementacion.Api.csproj"

# Compilamos y publicamos en modo Release
RUN dotnet publish "./Implementacion.Api.csproj" -c Release -o /app/publish

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render detecta el puerto 8080 automáticamente
EXPOSE 8080

# Comando de inicio
ENTRYPOINT ["dotnet", "Implementacion.Api.dll"]
