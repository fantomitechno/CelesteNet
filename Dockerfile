FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . ./
# Build and publish a release
RUN dotnet build -c Release -t:CelesteNet_Shared:Rebuild -t:CelesteNet_Server_ChatModule:Rebuild -t:CelesteNet_Server_FrontendModule:Rebuild -t:CelesteNet_Server_SqliteModule:Rebuild -t:CelesteNet_Server:Rebuild -t:CelesteNet_Server_APIModule:Rebuild

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/CelesteNet.Server/bin/Release/net8.0/ .
COPY CelesteNet.Server.FrontendModule/Content/frontend ./Content/frontend
ENTRYPOINT ["dotnet", "CelesteNet.Server.dll"]