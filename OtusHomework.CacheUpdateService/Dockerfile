FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OtusHomework.CacheUpdateService/OtusHomework.CacheUpdateService.csproj", "OtusHomework.CacheUpdateService/"]
COPY ["OtusHomework.Database/OtusHomework.Database.csproj", "OtusHomework.Database/"]
COPY ["OtusHomework.Kafka/OtusHomework.Kafka.csproj", "OtusHomework.Kafka/"]
RUN dotnet restore "./OtusHomework.CacheUpdateService/OtusHomework.CacheUpdateService.csproj"
COPY . .
WORKDIR "/src/OtusHomework.CacheUpdateService"
RUN dotnet build "./OtusHomework.CacheUpdateService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OtusHomework.CacheUpdateService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OtusHomework.CacheUpdateService.dll"]