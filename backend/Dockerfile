FROM ubuntu:latest AS build

ARG conan_user
ARG conan_password
ARG conan_remote

WORKDIR /backend
COPY . .
RUN chmod +x build.sh
RUN ./build.sh $conan_user $conan_password $conan_remote

LABEL stage=build Name=zwoo_backend_build

FROM ubuntu:latest AS runner
WORKDIR /backend
COPY --from=build ./backend/build/bin/ .
ENTRYPOINT ["./main"]

LABEL Name=zwoo_backend Version=0.0.1
