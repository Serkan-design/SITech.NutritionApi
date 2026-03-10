FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore SITech.NutritionApi/SITech.NutritionApi.csproj
RUN dotnet publish SITech.NutritionApi/SITech.NutritionApi.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /publish

COPY --from=build /publish .

EXPOSE 10000

ENTRYPOINT ["dotnet", "SITech.NutritionApi.dll"]
