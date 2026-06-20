FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/AzureCSharpRAGAssistant.Api/AzureCSharpRAGAssistant.Api.csproj src/AzureCSharpRAGAssistant.Api/
RUN dotnet restore src/AzureCSharpRAGAssistant.Api/AzureCSharpRAGAssistant.Api.csproj

COPY . .

RUN dotnet publish src/AzureCSharpRAGAssistant.Api/AzureCSharpRAGAssistant.Api.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AzureCSharpRAGAssistant.Api.dll"]