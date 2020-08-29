#docker run --name ignite1 --restart unless-stopped -d -it -p 47500-47520:47500-47520 -p 10800:10800 -m 250M -v ignite-volume:/persistence -e IGNITE_WORK_DIR=/persistence mustafasalih/fortnite:ignite
FROM apacheignite/ignite:2.8.1
EXPOSE 47500-47520

COPY custom-config.xml $IGNITE_HOME/custom-config.xml

RUN echo "127.0.0.1 linuxserver" >> /etc/hosts
RUN echo "Etc/UTC" > /etc/timezone

CMD $IGNITE_HOME/bin/ignite.sh $IGNITE_HOME/custom-config.xml