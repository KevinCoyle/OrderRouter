FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderRouter.Api/OrderRouter.Api.csproj", "OrderRouter.Api/"]
RUN dotnet restore "OrderRouter.Api/OrderRouter.Api.csproj"
COPY . .
WORKDIR "/src/OrderRouter.Api"
RUN dotnet build "./OrderRouter.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderRouter.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["OrderRouter.Api/ReferenceDocs/", "ReferenceDocs/"]
ENTRYPOINT ["dotnet", "OrderRouter.Api.dll"]
