FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LensLooker/LensLooker.csproj", "LensLooker/"]
RUN dotnet restore "LensLooker/LensLooker.csproj"
COPY . .
WORKDIR "/src/LensLooker"
RUN dotnet build "LensLooker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LensLooker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LensLooker.dll"]
