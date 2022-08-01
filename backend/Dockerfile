FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy project configs into /src/<project>/<config>.csproj
COPY ["./ZwooBackend/ZwooBackend.csproj", "./ZwooBackend/"]
COPY ["./GameLogic/GameLogic.csproj", "./GameLogic/"]
COPY ["./BackendHelper/BackendHelper.csproj", "./BackendHelper/"]
RUN dotnet restore "./ZwooBackend/ZwooBackend.csproj"

# copy all projects into /src (/src/<project>)
COPY . .
WORKDIR "/src/"

# build backend project
RUN dotnet build "./ZwooBackend/ZwooBackend.csproj" -c Release -o /app/build

FROM build AS publish
# publish project
RUN dotnet publish "./ZwooBackend/ZwooBackend.csproj" -c Release -o /app/publish

FROM base AS final
# run project
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "./ZwooBackend.dll"]