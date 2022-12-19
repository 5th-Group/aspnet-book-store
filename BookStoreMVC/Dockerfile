FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BookStoreMVC.csproj", "./"]
RUN dotnet restore "BookStoreMVC.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "BookStoreMVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStoreMVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStoreMVC.dll"]
