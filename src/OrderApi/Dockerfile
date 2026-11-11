FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/OrderApi/OrderApi.csproj", "src/OrderApi/"]
COPY ["src/SharedContracts/SharedContracts.csproj", "src/SharedContracts/"]
RUN dotnet restore "src/OrderApi/OrderApi.csproj"

COPY . .
WORKDIR "/src/src/OrderApi"
RUN dotnet publish "OrderApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderApi.dll"]