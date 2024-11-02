FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy and restore dependencies
COPY *.sln ./
COPY WidgetAndCo.Core/*.csproj ./WidgetAndCo.Core/
COPY WidgetAndCo.Data/*.csproj ./WidgetAndCo.Data/
COPY WidgetAndCo.Business/*.csproj ./WidgetAndCo.Business/
COPY WidgetAndCo.OrderProcessingFunction/*.csproj ./WidgetAndCo.OrderProcessingFunction/
COPY WidgetAndCo.ReviewProcessingFunction/*.csproj ./WidgetAndCo.ReviewProcessingFunction/
COPY WidgetAndCo/*.csproj ./WidgetAndCo/
RUN dotnet restore

# Copy all files and build the app
COPY . ./
WORKDIR /app/WidgetAndCo
RUN dotnet publish -c Release -o /app/out

# Use .NET 8 runtime image for execution
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "WidgetAndCo.dll"]