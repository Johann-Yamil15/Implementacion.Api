# Etapa 1: Compilar la aplicación
# Usamos la imagen del SDK de .NET 8.0 (puedes cambiar la versión si usas otra)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia los archivos del proyecto (.csproj) y restaura las dependencias primero
# Esto aprovecha el cache de Docker y hace las compilaciones más rápidas
COPY Implementacion.Api/*.csproj ./Implementacion.Api/
RUN dotnet restore ./Implementacion.Api/

# Copia todo el resto del código fuente
COPY . ./

# Publica la aplicación en modo Release
RUN dotnet publish ./Implementacion.Api/ -c Release -o out

# Etapa 2: Construir la imagen final de ejecución
# Usamos la imagen de ASP.NET Runtime que es más ligera
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# El comando para iniciar tu API
# Render usará el puerto que nos asigne, así que no es necesario EXPOSE aquí
ENTRYPOINT ["dotnet", "Implementacion.Api.dll"]