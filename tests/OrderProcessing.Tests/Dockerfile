FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/OrderProcessing.Tests/OrderProcessing.Tests.csproj", "tests/OrderProcessing.Tests/"]
COPY ["src/OrderWorker/OrderWorker.csproj", "src/OrderWorker/"]
COPY ["src/SharedContracts/SharedContracts.csproj", "src/SharedContracts/"]
RUN dotnet restore "tests/OrderProcessing.Tests/OrderProcessing.Tests.csproj"
COPY . .
WORKDIR "/src/tests/OrderProcessing.Tests"
RUN dotnet build "./OrderProcessing.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderProcessing.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessing.Tests.dll"]
