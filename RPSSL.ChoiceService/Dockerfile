# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RPSSL.ChoiceService/RPSSL.ChoiceService.csproj", "RPSSL.ChoiceService/"]
RUN dotnet restore "RPSSL.ChoiceService/RPSSL.ChoiceService.csproj"
COPY . .
WORKDIR "/src/RPSSL.ChoiceService"
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RPSSL.ChoiceService.dll"]
