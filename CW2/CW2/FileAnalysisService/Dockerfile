# 1) runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# 2) build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем оба сервиса сразу
COPY FileStoringService/ FileStoringService/
COPY FileAnalysisService/ FileAnalysisService/

# Восстанавливаем зависимости анализа (MSBuild найдёт FileStoringService как проект-зависимость)
RUN dotnet restore FileAnalysisService/FileAnalysisService.csproj

# Собираем анализатор
RUN dotnet build FileAnalysisService/FileAnalysisService.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# Перед публикацией убираем конфликтующий appsettings.json
RUN rm FileStoringService/appsettings.json

# 3) publish
FROM build AS publish
RUN dotnet publish FileAnalysisService/FileAnalysisService.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# 4) final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileAnalysisService.dll"]
