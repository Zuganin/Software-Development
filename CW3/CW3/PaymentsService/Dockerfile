# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PaymentsService/PaymentsService.csproj ./PaymentsService/
RUN dotnet restore "./PaymentsService/PaymentsService.csproj"

COPY PaymentsService/. ./PaymentsService/
WORKDIR /src/PaymentsService
RUN dotnet publish -c Release -o /app/publish

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "PaymentsService.dll"]
