FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OtusHomework.СacheWarmup/OtusHomework.СacheWarmup.csproj", "OtusHomework.СacheWarmup/"]
COPY ["OtusHomework.Database/OtusHomework.Database.csproj", "OtusHomework.Database/"]
RUN dotnet restore "./OtusHomework.СacheWarmup/OtusHomework.СacheWarmup.csproj"
COPY . .
WORKDIR "/src/OtusHomework.СacheWarmup"
RUN dotnet build "./OtusHomework.СacheWarmup.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OtusHomework.СacheWarmup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OtusHomework.СacheWarmup.dll"]