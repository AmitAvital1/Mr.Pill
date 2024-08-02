# Use .NET 8.0 SDK for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY ["LoginService/Login/Login.csproj", "LoginService/Login/"]
COPY ["Dto/Dtos.csproj", "Dto/"]
RUN dotnet restore "LoginService/Login/Login.csproj"

# Copy the rest of the application code
COPY . .

# Build the project
WORKDIR "/app/LoginService/Login"
RUN dotnet build "Login.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Login.csproj" -c Release -o /app/publish

# Use .NET 8.0 ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Wait for the SQL Server to be up and running, then apply migrations
CMD /bin/bash -c "while !</dev/tcp/db/1433; do sleep 1; done; dotnet Login.dll"
