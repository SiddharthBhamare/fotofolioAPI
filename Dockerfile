# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy solution and project file
COPY fotofolioAPI.sln ./
COPY fotofolioAPI.csproj ./

# Restore the dependencies
RUN dotnet restore "fotofolioAPI.csproj"

# Copy everything else
COPY . ./

# Build the application
RUN dotnet publish "fotofolioAPI.csproj" -c Release -o /app/publish

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "fotofolioAPI.dll"]
