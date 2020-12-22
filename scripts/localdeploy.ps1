# build and deploy from your local machine
# ...becuase github actions and docker hub are
# incapable of building an arm64 image that works.
# ...or at least I'm incapable of making them do so.

write-host "** Building Image **"
docker build -t snowjallen/homespeakerserver -f ./HomeSpeaker.Server/Dockerfile .

write-host "** Pushing Image **"
docker push snowjallen/homespeakerserver

write-host "** Copy latest restart.ps1 and docker-compose.yml files **"
scp scripts/restart.ps1 pi@192.168.1.20:~/scripts/
scp scripts/docker-compose.yml pi@192.168.1.20:~/scripts/

write-host "** Restarting docker-compose on rpi4 **"
ssh rpi4 'cd scripts && pwsh ./restart.ps1 -tag latest'