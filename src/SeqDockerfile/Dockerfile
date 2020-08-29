#docker run --name seq --restart unless-stopped -d -it -p 127.0.0.1:5341:80 -m 200M --cpus="1" -v seq-volume:/data -e ACCEPT_EULA=Y mustafasalih/fortnite:seq FROM datalust/seq:latest

COPY Seq.json /data/Seq.json

RUN echo "127.0.0.1 linuxserver" >> /etc/hosts
RUN echo "Etc/UTC" > /etc/timezone
