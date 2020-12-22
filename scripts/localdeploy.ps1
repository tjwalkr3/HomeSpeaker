# build and deploy from your local machine
# ...becuase github actions and docker hub are 
# incapable of building an arm64 image that works.
# ...or at least I'm incapable of making them do so.

docker build -t snowjallen/homespeakerserver -f ./HomeSpeaker.Server/Dockerfile .
docker push snowjallen/homespeakerserver
ssh rpi4 'cd scripts && pwsh ./restart.ps1 -tag latest'