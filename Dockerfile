FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY ["DataEF/DataEF.csproj", "DataEF/DataEF.csproj"]
COPY ["TaxLedgerAPI/TaxLedgerAPI.csproj", "TaxLedgerAPI/TaxLedgerAPI.csproj"]
RUN dotnet restore 'TaxLedgerAPI/TaxLedgerAPI.csproj'

COPY ["DataEF", "DataEF"]
COPY ["TaxLedgerAPI", "TaxLedgerAPI"]
RUN dotnet build 'TaxLedgerAPI/TaxLedgerAPI.csproj' -c Release -o /app/build

FROM build AS publish
RUN dotnet publish 'TaxLedgerAPI/TaxLedgerAPI.csproj' -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaxLedgerAPI.dll"]