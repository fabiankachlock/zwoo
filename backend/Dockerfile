FROM ghcr.io/zwoo-hq/docker-base:v1.0.0 AS builder

WORKDIR /src
COPY ./zwooc.config.json ./

WORKDIR /src/backend

COPY ./backend/Directories.Build.props ./
COPY ./backend/Zwoo.Backend/Zwoo.Backend.csproj ./Zwoo.Backend/
COPY ./backend/Zwoo.GameEngine/Zwoo.GameEngine.csproj ./Zwoo.GameEngine/
COPY ./backend/BackendHelper/BackendHelper.csproj ./BackendHelper/
COPY ./backend/Zwoo.Database/Zwoo.Database.csproj ./Zwoo.Database/

RUN dotnet restore ./Zwoo.Backend/Zwoo.Backend.csproj

COPY ./backend .

RUN zwooc build -e setup:version:be server -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# version override
ARG version_override
ENV ZWOO_VERSION_OVERRIDE=${version_override}

# run project
WORKDIR /app
COPY --from=builder /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "./Zwoo.Backend.dll"]
