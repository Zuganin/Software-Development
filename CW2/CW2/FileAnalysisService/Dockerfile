# 1) runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2) build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем оба сервиса сразу
COPY FileStoringService/ FileStoringService/
COPY FileAnalisysService/ FileAnalisysService/

# Восстанавливаем зависимости анализа (MSBuild найдёт FileStoringService как проект-зависимость)
RUN dotnet restore FileAnalisysService/FileAnalisysService.csproj

# Собираем анализатор
RUN dotnet build FileAnalisysService/FileAnalisysService.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# Перед публикацией убираем конфликтующий appsettings.json
RUN rm FileStoringService/appsettings.json

# 3) publish
FROM build AS publish
RUN dotnet publish FileAnalisysService/FileAnalisysService.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# 4) final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileAnalisysService.dll"]
