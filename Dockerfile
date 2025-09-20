# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy solution file
COPY ["TodoListApp.sln", "."]

# Copy project files
COPY ["src/TodoListApp.Api/TodoListApp.Api.csproj", "src/TodoListApp.Api/"]
COPY ["src/TodoListApp.Application/TodoListApp.Application.csproj", "src/TodoListApp.Application/"]
COPY ["src/TodoListApp.Domain/TodoListApp.Domain.csproj", "src/TodoListApp.Domain/"]
COPY ["src/TodoListApp.Infrastructure/TodoListApp.Infrastructure.csproj", "src/TodoListApp.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/TodoListApp.Api/TodoListApp.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/TodoListApp.Api"
RUN dotnet build "TodoListApp.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TodoListApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Expose the port
EXPOSE 8080
EXPOSE 8081

# Set the entry point
ENTRYPOINT ["dotnet", "TodoListApp.Api.dll"]
