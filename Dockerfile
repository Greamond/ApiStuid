#See https://aka.ms/customizecontainer  to learn how to customize your debug container

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj-файл из подпапки ApiStuid
COPY ["ApiStuid/ApiStuid.csproj", "ApiStuid/"]

# Восстанавливаем зависимости
RUN dotnet restore "./ApiStuid/ApiStuid.csproj"

# Копируем весь код проекта в /src
COPY . .

# Переходим в папку проекта и собираем
WORKDIR "/src/ApiStuid"
RUN dotnet build "ApiStuid.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApiStuid.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiStuid.dll"]