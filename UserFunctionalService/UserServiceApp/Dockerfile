# Use .NET 8.0 SDK for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY ["UserFunctionalService/UserServiceApp/UserServiceApp.csproj", "UserFunctionalService/UserServiceApp/"]
COPY ["Dto/Dtos.csproj", "Dto/"]
RUN dotnet restore "UserFunctionalService/UserServiceApp/UserServiceApp.csproj"

# Copy the rest of the application code
COPY . .

# Build the project
WORKDIR "/app/UserFunctionalService/UserServiceApp"
RUN dotnet build "UserServiceApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserServiceApp.csproj" -c Release -o /app/publish

# Use .NET 8.0 ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

FROM build as migrations
RUN dotnet tool install --version 8.0.4 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Wait for the SQL Server to be up and running, then apply migrations
WORKDIR /app/UserFunctionalService/UserServiceApp
CMD /bin/bash -c "while !</dev/tcp/db/1433; do sleep 1; done; dotnet ef database update; dotnet /app/build/UserServiceApp.dll"
