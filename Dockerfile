FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app

ARG SONAR_HOST
ARG SONAR_TOKEN
ARG PUBLISHER_KEY
ARG WEBAPI_KEY
ARG STEAMID

# Install sonarScanner
RUN dotnet tool install --global dotnet-sonarscanner
ENV DOTNET_ROLL_FORWARD=Major
ENV PATH="${PATH}:/root/.dotnet/tools"

# Install sonarScanner dependencies
RUN apt-get update
# Next line fix openjdk11 install
RUN mkdir -p /usr/share/man/man1
RUN apt install openjdk-11-jdk jq -y
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs

# Build Project
COPY . ./

RUN jq ".PUBLISHER_KEY = \"$PUBLISHER_KEY\"" Dysnomia.Common.WebAPIWrapper.Test/appsettings.json > tmp.appsettings.json && mv tmp.appsettings.json Dysnomia.Common.WebAPIWrapper.Test/appsettings.json
RUN jq ".WEBAPI_KEY = \"$WEBAPI_KEY\"" Dysnomia.Common.WebAPIWrapper.Test/appsettings.json > tmp.appsettings.json && mv tmp.appsettings.json Dysnomia.Common.WebAPIWrapper.Test/appsettings.json
RUN jq ".STEAMID = \"$STEAMID\"" Dysnomia.Common.WebAPIWrapper.Test/appsettings.json > tmp.appsettings.json && mv tmp.appsettings.json Dysnomia.Common.WebAPIWrapper.Test/appsettings.json

RUN dotnet sonarscanner begin /k:"dysnomia-common-webapiwrapper" /d:sonar.host.url="$SONAR_HOST" /d:sonar.login="$SONAR_TOKEN" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" /d:sonar.coverage.exclusions="**Test*.cs"
RUN dotnet restore Dysnomia.Common.WebAPIWrapper.sln --ignore-failed-sources /p:EnableDefaultItems=false
RUN dotnet build Dysnomia.Common.WebAPIWrapper.sln --no-restore -c Release -o out
RUN dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
RUN dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"