# Use .NET 8.0 SDK for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY ["PushNotificationService/PushNotificationService.csproj", "PushNotificationService/"]
COPY ["Dto/Dtos.csproj", "Dto/"]
RUN dotnet restore "PushNotificationService/PushNotificationService.csproj"

# Copy the rest of the application code
COPY . .

# Build the project
WORKDIR "/app/PushNotificationService"
RUN dotnet build "PushNotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PushNotificationService.csproj" -c Release -o /app/publish

# Use .NET 8.0 ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Start the application
CMD dotnet PushNotificationService.dll



# pushnotification:
#     build:
#       context: .
#       dockerfile: PushNotificationService/Dockerfile
#     ports:
#       - "5201:8080"
#     restart: always
#     networks:
#       - mynetwork