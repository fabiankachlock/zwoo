FROM ghcr.io/zwoo-hq/docker-base:v1.0.0 AS builder

WORKDIR /src
COPY ./zwooc.config.json ./

# setup frontend
WORKDIR /src/frontend
COPY ./frontend/package.json ./
COPY ./frontend/pnpm-lock.yaml ./

RUN corepack enable
RUN pnpm install --frozen-lockfile


# setup wasm related backend stuff
WORKDIR /src/backend
COPY ./backend/Zwoo.GameEngine.Wasm/Zwoo.GameEngine.Wasm.csproj ./Zwoo.GameEngine.Wasm/
COPY ./backend/Zwoo.GameEngine/Zwoo.GameEngine.csproj ./Zwoo.GameEngine/

RUN dotnet workload install wasm-tools \
    && dotnet restore ./Zwoo.GameEngine.Wasm/Zwoo.GameEngine.Wasm.csproj


### build wasm project
COPY ./backend .

RUN zwooc build wasm

### Build Fronten
WORKDIR /src/frontend
COPY ./frontend .

ARG version_override
ARG profile=prod

RUN VUE_APP_VERSION_OVERRIDE=$version_override zwooc build -e wasm -e setup:version:fe "$profile"


### Production Stage
FROM nginx:stable-alpine AS prod
COPY --from=builder /src/frontend/dist /usr/share/nginx/html
COPY ./frontend/nginx.conf /etc/nginx/

CMD ["nginx", "-g", "daemon off;"]
