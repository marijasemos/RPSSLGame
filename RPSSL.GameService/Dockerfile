# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RPSSL.GameService/RPSSL.GameService.csproj", "RPSSL.GameService/"]
RUN dotnet restore "RPSSL.GameService/RPSSL.GameService.csproj"
COPY . .
WORKDIR "/src/RPSSL.GameService"
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RPSSL.GameService.dll"]
