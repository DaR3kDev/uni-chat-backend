# =========================
# RUNTIME (final image)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

# =========================
# BUILD IMAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# CSPROJ en la raíz del contexto
COPY ["uni-chat-backend.csproj", "./"]

RUN dotnet restore "uni-chat-backend.csproj"

# copiar todo el código
COPY . .

RUN dotnet build "uni-chat-backend.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# =========================
# PUBLISH IMAGE
# =========================
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "uni-chat-backend.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# FINAL RUNTIME
# =========================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "uni-chat-backend.dll"]