FROM ghcr.io/zwoo-hq/docker-base:v0.2.0 AS builder

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

RUN zwooc build server -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# version override
ARG version_override
ENV ZWOO_VERSION_OVERRIDE=${version_override}

# run project
WORKDIR /app
COPY --from=builder /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "./Zwoo.Backend.dll"]
