FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/OrderWorker/OrderWorker.csproj", "src/OrderWorker/"]
COPY ["src/SharedContracts/SharedContracts.csproj", "src/SharedContracts/"]
RUN dotnet restore "src/OrderWorker/OrderWorker.csproj"

COPY . .
WORKDIR "/src/src/OrderWorker"
RUN dotnet publish "OrderWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderWorker.dll"]