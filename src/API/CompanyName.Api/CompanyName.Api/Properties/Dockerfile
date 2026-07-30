FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/CompanyName.Api/CompanyName.Api.csproj", "API/CompanyName.Api/"]
COPY ["Common/CompanyName.Common.Application/CompanyName.Common.Application.csproj", "Common/CompanyName.Common.Application/"]
COPY ["Common/CompanyName.Common.Domain/CompanyName.Common.Domain.csproj", "Common/CompanyName.Common.Domain/"]
COPY ["Common/CompanyName.Common.Infrastructure/CompanyName.Common.Infrastructure.csproj", "Common/CompanyName.Common.Infrastructure/"]
COPY ["Common/CompanyName.Common.Presentation/CompanyName.Common.Presentation.csproj", "Common/CompanyName.Common.Presentation/"]
COPY ["Modules/Attendance/CompanyName.Modules.Attendance.Application/CompanyName.Modules.Attendance.Application.csproj", "Modules/Attendance/CompanyName.Modules.Attendance.Application/"]
COPY ["Modules/Attendance/CompanyName.Modules.Attendance.Domain/CompanyName.Modules.Attendance.Domain.csproj", "Modules/Attendance/CompanyName.Modules.Attendance.Domain/"]
COPY ["Modules/Attendance/CompanyName.Modules.Attendance.Infrastructure/CompanyName.Modules.Attendance.Infrastructure.csproj", "Modules/Attendance/CompanyName.Modules.Attendance.Infrastructure/"]
COPY ["Modules/Attendance/CompanyName.Modules.Attendance.IntegrationEvents/CompanyName.Modules.Attendance.IntegrationEvents.csproj", "Modules/Attendance/CompanyName.Modules.Attendance.IntegrationEvents/"]
COPY ["Modules/Attendance/CompanyName.Modules.Attendance.Presentation/CompanyName.Modules.Attendance.Presentation.csproj", "Modules/Attendance/CompanyName.Modules.Attendance.Presentation/"]
COPY ["Modules/Events/CompanyName.Modules.Events.Application/CompanyName.Modules.Events.Application.csproj", "Modules/Events/CompanyName.Modules.Events.Application/"]
COPY ["Modules/Events/CompanyName.Modules.Events.Domain/CompanyName.Modules.Events.Domain.csproj", "Modules/Events/CompanyName.Modules.Events.Domain/"]
COPY ["Modules/Events/CompanyName.Modules.Events.Infrastructure/CompanyName.Modules.Events.Infrastructure.csproj", "Modules/Events/CompanyName.Modules.Events.Infrastructure/"]
COPY ["Modules/Events/CompanyName.Modules.Events.IntegrationEvents/CompanyName.Modules.Events.IntegrationEvents.csproj", "Modules/Events/CompanyName.Modules.Events.IntegrationEvents/"]
COPY ["Modules/Events/CompanyName.Modules.Events.Presentation/CompanyName.Modules.Events.Presentation.csproj", "Modules/Events/CompanyName.Modules.Events.Presentation/"]
COPY ["Modules/Ticketing/CompanyName.Modules.Ticketing.Application/CompanyName.Modules.Ticketing.Application.csproj", "Modules/Ticketing/CompanyName.Modules.Ticketing.Application/"]
COPY ["Modules/Ticketing/CompanyName.Modules.Ticketing.Domain/CompanyName.Modules.Ticketing.Domain.csproj", "Modules/Ticketing/CompanyName.Modules.Ticketing.Domain/"]
COPY ["Modules/Ticketing/CompanyName.Modules.Ticketing.Infrastructure/CompanyName.Modules.Ticketing.Infrastructure.csproj", "Modules/Ticketing/CompanyName.Modules.Ticketing.Infrastructure/"]
COPY ["Modules/Ticketing/CompanyName.Modules.Ticketing.IntegrationEvents/CompanyName.Modules.Ticketing.IntegrationEvents.csproj", "Modules/Ticketing/CompanyName.Modules.Ticketing.IntegrationEvents/"]
COPY ["Modules/Ticketing/CompanyName.Modules.Ticketing.Presentation/CompanyName.Modules.Ticketing.Presentation.csproj", "Modules/Ticketing/CompanyName.Modules.Ticketing.Presentation/"]
COPY ["Modules/Users/CompanyName.Modules.Users.Application/CompanyName.Modules.Users.Application.csproj", "Modules/Users/CompanyName.Modules.Users.Application/"]
COPY ["Modules/Users/CompanyName.Modules.Users.Domain/CompanyName.Modules.Users.Domain.csproj", "Modules/Users/CompanyName.Modules.Users.Domain/"]
COPY ["Modules/Users/CompanyName.Modules.Users.Infrastructure/CompanyName.Modules.Users.Infrastructure.csproj", "Modules/Users/CompanyName.Modules.Users.Infrastructure/"]
COPY ["Modules/Users/CompanyName.Modules.Users.IntegrationEvents/CompanyName.Modules.Users.IntegrationEvents.csproj", "Modules/Users/CompanyName.Modules.Users.IntegrationEvents/"]
COPY ["Modules/Users/CompanyName.Modules.Users.Presentation/CompanyName.Modules.Users.Presentation.csproj", "Modules/Users/CompanyName.Modules.Users.Presentation/"]
RUN dotnet restore "./API/CompanyName.Api/CompanyName.Api.csproj"
COPY . .
WORKDIR "/src/API/CompanyName.Api"
RUN dotnet build "./CompanyName.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CompanyName.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyName.Api.dll"]