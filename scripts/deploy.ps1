
docker build -t 192.168.1.133:5555/homespeakerwebclient:arm32v7 -f .\HomeSpeaker.Client.Web\Dockerfile .
docker push 192.168.1.133:5555/homespeakerwebclient:arm32v7

docker build -t 192.168.1.133:5555/homespeakerserver:arm32v7 -f .\HomeSpeaker.Server\Dockerfile .
docker push 192.168.1.133:5555/homespeakerserver:arm32v7
