FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set the environment variable
ENV DATA_FILE_PATH=../../../../src/Infrastructure/OrderManagement/Data/order_books_data

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/MetaExchange/MetaExchange.Api/MetaExchange.Api.csproj", "Presentation/MetaExchange/MetaExchange.Api/"]
RUN dotnet restore "./Presentation/MetaExchange/MetaExchange.Api/./MetaExchange.Api.csproj"
COPY . .
WORKDIR "/src/Presentation/MetaExchange/MetaExchange.Api"
RUN dotnet build "./MetaExchange.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MetaExchange.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MetaExchange.Api.dll"]