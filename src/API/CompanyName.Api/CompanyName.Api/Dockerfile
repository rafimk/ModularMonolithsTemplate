# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/API/CompanyName.Api/CompanyName.Api/CompanyName.Api.csproj", "src/API/CompanyName.Api/CompanyName.Api/"]
COPY ["src/Common/CompanyName.Common.Infrastructure/CompanyName.Common.Infrastructure.csproj", "src/Common/CompanyName.Common.Infrastructure/"]
COPY ["src/Common/CompanyName.Common.Application/CompanyName.Common.Application.csproj", "src/Common/CompanyName.Common.Application/"]
COPY ["src/Common/CompanyName.Common.Domain/CompanyName.Common.Domain.csproj", "src/Common/CompanyName.Common.Domain/"]
COPY ["src/Common/CompanyName.Common.Presentation/CompanyName.Common.Presentation.csproj", "src/Common/CompanyName.Common.Presentation/"]
COPY ["src/Modules/Users/CompanyName.Modules.Users.Infrastructure/CompanyName.Modules.Users.Infrastructure.csproj", "src/Modules/Users/CompanyName.Modules.Users.Infrastructure/"]
COPY ["src/Modules/Users/CompanyName.Modules.Users.Application/CompanyName.Modules.Users.Application.csproj", "src/Modules/Users/CompanyName.Modules.Users.Application/"]
COPY ["src/Modules/Users/CompanyName.Modules.Users.Domain/CompanyName.Modules.Users.Domain.csproj", "src/Modules/Users/CompanyName.Modules.Users.Domain/"]
COPY ["src/Modules/Users/CompanyName.Modules.Users.IntegrationEvents/CompanyName.Modules.Users.IntegrationEvents.csproj", "src/Modules/Users/CompanyName.Modules.Users.IntegrationEvents/"]
COPY ["src/Modules/Users/CompanyName.Modules.Users.Presentation/CompanyName.Modules.Users.Presentation.csproj", "src/Modules/Users/CompanyName.Modules.Users.Presentation/"]
RUN dotnet restore "./src/API/CompanyName.Api/CompanyName.Api/CompanyName.Api.csproj"
COPY . .
WORKDIR "/src/src/API/CompanyName.Api/CompanyName.Api"
RUN dotnet build "./CompanyName.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CompanyName.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyName.Api.dll"]
