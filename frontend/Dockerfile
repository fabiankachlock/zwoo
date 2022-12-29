### Frontend Setup
FROM node:16-alpine as base

# setup frontend
WORKDIR /src/frontend
COPY ./frontend/package.json ./
COPY ./frontend/yarn.lock ./

RUN yarn

#copy frontend source
COPY ./frontend/ ./

### Build WASM
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dotnet-builder

# copy sources
COPY --from=base /src ./src
WORKDIR /src/backend

# copy project configs into /src/backend/<project>/<config>.csproj
COPY ["./backend/ZwooWasmAdapter/ZwooWasmAdapter.csproj", "./ZwooWasmAdapter/"]
COPY ["./backend/GameLogic/GameLogic.csproj", "./GameLogic/"]
RUN dotnet restore "./ZwooWasmAdapter/ZwooWasmAdapter.csproj"

# copy all backend projects into /src/backend (/src/backend/<project>)
COPY ./backend .

# build wasm project
RUN dotnet publish -c Release -p:RunAOTCompilation=true ./ZwooWasmAdapter/ZwooWasmAdapter.csproj

### Build Frontend
FROM node:16-alpine as builder

# copy spurces + compiled wasm
COPY --from=dotnet-builder /src ./src
WORKDIR /src/frontend

# build frontend
RUN yarn build

### Production Stage
FROM nginx:stable-alpine as prod
COPY --from=builder /src/frontend/dist /usr/share/nginx/html
COPY ./frontend/nginx.conf /etc/nginx/

CMD ["nginx", "-g", "daemon off;"]
