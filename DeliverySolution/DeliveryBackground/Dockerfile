FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DeliveryBackground/DeliveryBackground.csproj", "DeliveryBackground/"]
RUN dotnet restore "DeliveryBackground/DeliveryBackground.csproj"
COPY . .
WORKDIR "/src/DeliveryBackground"
RUN dotnet build "DeliveryBackground.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliveryBackground.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliveryBackground.dll"]
