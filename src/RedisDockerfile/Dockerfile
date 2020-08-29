#docker run --name redissv --log-opt max-size=25m --log-opt max-file=4 --restart unless-stopped -d -it --net=host -p 6379:6379 mustafasalih/fortnite:redis redis-server /etc/redis/redis_Remote.conf --appendonly yes
#docker run --name redissv --restart unless-stopped -d -it --net=host -p 6379:6379 mustafasalih/fortnite:redis redis-server /etc/redis/redis_Local.conf --appendonly yes
FROM redis:latest
EXPOSE 6379

COPY redis_Remote.conf /etc/redis/
COPY redis_Local.conf /etc/redis/
RUN echo "127.0.0.1 linuxserver" >> /etc/hosts
RUN echo "Etc/UTC" > /etc/timezone