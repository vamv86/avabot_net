# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Etapa build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el backend
COPY . .

# Restaurar y compilar
RUN dotnet restore "AiAgentApi.csproj"
RUN dotnet build "AiAgentApi.csproj" -c Release -o /app/build

# Publicar
FROM build AS publish
RUN dotnet publish "AiAgentApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AiAgentApi.dll"]
