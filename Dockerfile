# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SiTech.NutritionApi/SiTech.NutritionApi.csproj SiTech.NutritionApi/
RUN dotnet restore SiTech.NutritionApi/SiTech.NutritionApi.csproj

COPY . .
RUN dotnet publish SiTech.NutritionApi/SiTech.NutritionApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SiTech.NutritionApi.dll"]