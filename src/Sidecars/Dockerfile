FROM envoyproxy/envoy:v1.9.0
RUN apt-get update && apt-get -q install -y curl
RUN apt-get -q install -y net-tools
RUN apt-get -q install -y dnsutils
RUN apt-get -q install -y iputils-ping
RUN apt-get -q install -y inetutils-traceroute
RUN apt-get -q install -y tcpdump
COPY default-sidecar.yaml /etc/default-sidecar.yaml
ENTRYPOINT envoy -c /etc/default-sidecar.yaml