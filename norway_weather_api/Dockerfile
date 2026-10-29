FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY norway_weather_api.csproj ./

RUN dotnet restore norway_weather_api.csproj

COPY . .

RUN dotnet publish norway_weather_api.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/certs
COPY certs/global-bundle.pem /app/certs/global-bundle.pem

EXPOSE 8080

ENTRYPOINT ["dotnet", "norway_weather_api.dll"]