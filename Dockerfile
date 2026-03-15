# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SplitwiseClone.Api/SplitwiseClone.Api.csproj", "SplitwiseClone.Api/"]
COPY ["SplitwiseClone.Application/SplitwiseClone.Application.csproj", "SplitwiseClone.Application/"]
COPY ["SplitwiseClone.Core/SplitwiseClone.Domain.csproj", "SplitwiseClone.Core/"]
COPY ["SplitwiseClone.Persistence/SplitwiseClone.Persistence.csproj", "SplitwiseClone.Persistence/"]

RUN dotnet restore "SplitwiseClone.Api/SplitwiseClone.Api.csproj"

COPY . .
WORKDIR "/src/SplitwiseClone.Api"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SplitwiseClone.Api.dll"]