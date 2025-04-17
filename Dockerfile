# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the solution file into the container
COPY FotofolioAPI.sln ./

# Copy the project file into the container
COPY FotofolioAPI/FotofolioAPI.csproj FotofolioAPI/

# Restore the dependencies for the project
RUN dotnet restore "FotofolioAPI/FotofolioAPI.csproj"

# Copy the rest of the application files into the container
COPY . .

# Publish the application to the /app folder in the container
RUN dotnet publish "FotofolioAPI/FotofolioAPI.csproj" -c Release -o /app

# Set the base image to use for running the application (runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory in the runtime image
WORKDIR /app

# Copy the published application from the build container
COPY --from=build /app .

# Expose port 80 to allow traffic to the container
EXPOSE 80

# Define the entry point for the application (run the API)
ENTRYPOINT ["dotnet", "FotofolioAPI.dll"]
