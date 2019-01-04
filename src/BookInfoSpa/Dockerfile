FROM microsoft/aspnetcore:2.0 AS base
RUN apt-get update && apt-get install -y nodejs
RUN apt-get update && apt-get -q install -y curl
RUN apt-get -q install -y net-tools
RUN apt-get -q install -y dnsutils
RUN apt-get -q install -y iputils-ping
RUN apt-get -q install -y inetutils-traceroute
RUN ln -s /usr/bin/nodejs /usr/bin/node
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY BookInfoSpa/BookInfoSpa.csproj BookInfoSpa/
RUN dotnet restore BookInfoSpa/BookInfoSpa.csproj
COPY . .
WORKDIR /src/BookInfoSpa
RUN dotnet build BookInfoSpa.csproj -c Debug -o /app
RUN dotnet publish BookInfoSpa.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "BookInfoSpa.dll"]
