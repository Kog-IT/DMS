FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/DMS.Web.Host/DMS.Web.Host.csproj", "src/DMS.Web.Host/"]
COPY ["src/DMS.Web.Core/DMS.Web.Core.csproj", "src/DMS.Web.Core/"]
COPY ["src/DMS.Application/DMS.Application.csproj", "src/DMS.Application/"]
COPY ["src/DMS.Core/DMS.Core.csproj", "src/DMS.Core/"]
COPY ["src/DMS.EntityFrameworkCore/DMS.EntityFrameworkCore.csproj", "src/DMS.EntityFrameworkCore/"]
WORKDIR "/src/src/DMS.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/DMS.Web.Host", "src/DMS.Web.Host"]
COPY ["src/DMS.Web.Core", "src/DMS.Web.Core"]
COPY ["src/DMS.Application", "src/DMS.Application"]
COPY ["src/DMS.Core", "src/DMS.Core"]
COPY ["src/DMS.EntityFrameworkCore", "src/DMS.EntityFrameworkCore"]
WORKDIR "/src/src/DMS.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "DMS.Web.Host.dll"]
