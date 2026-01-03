# multi-stage build for ASP.NET Core 3.1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
# Production environment variable (adjust if needed)
ENV ASPNETCORE_ENVIRONMENT=Production

# copy published output
COPY --from=build /app/out ./

# Expose port 80
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "ShoppingListAPI.dll"]
