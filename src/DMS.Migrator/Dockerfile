FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["src/DMS.Migrator/DMS.Migrator.csproj", "src/DMS.Migrator/"]
COPY ["src/DMS.Core/DMS.Core.csproj", "src/DMS.Core/"]
COPY ["src/DMS.EntityFrameworkCore/DMS.EntityFrameworkCore.csproj", "src/DMS.EntityFrameworkCore/"]
WORKDIR "/src/src/DMS.Migrator"
RUN dotnet restore

WORKDIR /src
COPY ["src/DMS.Migrator", "src/DMS.Migrator"]
COPY ["src/DMS.Core", "src/DMS.Core"]
COPY ["src/DMS.EntityFrameworkCore", "src/DMS.EntityFrameworkCore"]
WORKDIR "/src/src/DMS.Migrator"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "DMS.Migrator.dll"]
