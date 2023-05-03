FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Delivery.API/Delivery.API.csproj", "Delivery.API/"]
RUN dotnet restore "Delivery.API/Delivery.API.csproj"
COPY . .
WORKDIR "/src/Delivery.API"
RUN dotnet build "Delivery.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Delivery.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Delivery.API.dll"]
