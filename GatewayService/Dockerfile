FROM mcr.microsoft.com/dotnet/core/sdk:2.2

COPY bin/Debug/netcoreapp2.2/publish app
WORKDIR app
ENTRYPOINT ["dotnet", "GatewayService.dll"]