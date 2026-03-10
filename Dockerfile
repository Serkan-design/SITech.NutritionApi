FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore SITech.NutritionApi/SITech.NutritionApi.csproj
RUN dotnet publish SITech.NutritionApi/SITech.NutritionApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

ENTRYPOINT ["dotnet","SITech.NutritionApi.dll"]
