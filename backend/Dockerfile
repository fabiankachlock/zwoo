FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy project configs into /src/<project>/<config>.csproj
COPY ["./Zwoo.Backend/Zwoo.Backend.csproj", "./Zwoo.Backend/"]
COPY ["./Zwoo.GameEngine/Zwoo.GameEngine.csproj", "./Zwoo.GameEngine/"]
COPY ["./BackendHelper/BackendHelper.csproj", "./BackendHelper/"]
COPY ["./Zwoo.Database/Zwoo.Database.csproj", "./Zwoo.Database/"]
RUN dotnet restore "./Zwoo.Backend/Zwoo.Backend.csproj"

# copy all projects into /src (/src/<project>)
COPY . .
WORKDIR "/src/"

# build backend project
RUN dotnet build "./Zwoo.Backend/Zwoo.Backend.csproj" -c Release -o /app/build

FROM build AS publish
# publish project
RUN dotnet publish "./Zwoo.Backend/Zwoo.Backend.csproj" -c Release -o /app/publish

FROM base AS final

# version override
ARG version_override
ENV ZWOO_VERSION_OVERRIDE=${version_override}

# run project
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "./Zwoo.Backend.dll"]
