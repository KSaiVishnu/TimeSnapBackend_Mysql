# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TimeSnapBackend_Mysql.sln ./
COPY TimeSnapBackend_Mysql/TimeSnapBackend_Mysql.csproj TimeSnapBackend_Mysql/
RUN dotnet restore TimeSnapBackend_Mysql/TimeSnapBackend_Mysql.csproj

# Copy the full project and build
COPY TimeSnapBackend_Mysql/. TimeSnapBackend_Mysql/
WORKDIR /src/TimeSnapBackend_Mysql
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "TimeSnapBackend_Mysql.dll"]
