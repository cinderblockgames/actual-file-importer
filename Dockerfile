FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

WORKDIR /app
COPY ./ ./
WORKDIR /app/AFI
RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/runtime:6.0

WORKDIR /app
COPY --from=build-env /app/AFI/out .

RUN apt-get update && \
    apt-get install -y dumb-init nodejs npm && \
    npm i @actual-app/api
    

# both of these are required
ENV SERVER_URL=
ENV BUDGET_SYNC_ID=

# one of these two is required
ENV SERVER_PASSWORD=
ENV SERVER_PASSWORD_FILE=

# required but defaulted
ENV IMPORT_BASE_PATH=/import


ENTRYPOINT ["/usr/bin/dumb-init", "--"]
CMD [ "dotnet", "/app/AFI.dll" ]