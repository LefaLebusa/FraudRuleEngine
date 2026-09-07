FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY FraudRuleEngine.sln .
COPY src/FraudRuleEngine.Core/FraudRuleEngine.Core.csproj src/FraudRuleEngine.Core/
COPY src/FraudRuleEngine.Application/FraudRuleEngine.Application.csproj src/FraudRuleEngine.Application/
COPY src/FraudRuleEngine.Infrastructure/FraudRuleEngine.Infrastructure.csproj src/FraudRuleEngine.Infrastructure/
COPY src/FraudRuleEngine.API/FraudRuleEngine.API.csproj src/FraudRuleEngine.API/

RUN dotnet restore src/FraudRuleEngine.API/FraudRuleEngine.API.csproj

COPY . .
RUN dotnet publish src/FraudRuleEngine.API/FraudRuleEngine.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FraudRuleEngine.API.dll"]
