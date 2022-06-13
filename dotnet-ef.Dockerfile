FROM mcr.microsoft.com/dotnet/sdk:6.0
RUN dotnet tool install dotnet-ef
ENTRYPOINT ["/root/.dotnet/tools/dotnet-ef", "migrations", "add"]