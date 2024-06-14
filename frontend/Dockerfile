FROM zwoo-hq/docker-base as base

# setup frontend
WORKDIR /src/frontend
COPY ./frontend/package.json ./
COPY ./frontend/yarn.lock ./

RUN yarn

# setup backend
WORKDIR /src/backend

COPY ./backend/Zwoo.GameEngine.Wasm/Zwoo.GameEngine.Wasm.csproj ./Zwoo.GameEngine.Wasm/
COPY ./backend/Zwoo.GameEngine/Zwoo.GameEngine.csproj ./Zwoo.GameEngine/
RUN dotnet restore ./Zwoo.GameEngine.Wasm/Zwoo.GameEngine.Wasm.csproj

COPY ./backend .

# build wasm project
RUN dotnet publish -c Release -p:RunAOTCompilation=true ./Zwoo.GameEngine.Wasm/Zwoo.GameEngine.Wasm.csproj

# copy spurces + compiled wasm
WORKDIR /src/frontend

# version override
ARG version_override

# configure command for building the frontend
ARG build_command=build-fe-only

# build frontend
RUN VUE_APP_VERSION_OVERRIDE=$version_override yarn "$build_command"

### Production Stage
FROM nginx:stable-alpine as prod
COPY --from=base /src/frontend/dist /usr/share/nginx/html
COPY ./frontend/nginx.conf /etc/nginx/

CMD ["nginx", "-g", "daemon off;"]
