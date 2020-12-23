# build and deploy from your local machine
# ...becuase github actions and docker hub are
# incapable of building an arm64 image that works.
# ...or at least I'm incapable of making them do so.
param(
    [switch]$skipBuild,
    [string]$server=192.168.1.20
)

$version = 0;
if(test-path "version.txt") {
    $version = [int](get-content "version.txt")
}

if($skipBuild -eq $false) {
    ($version++) | set-content "version.txt"
    $tag = "v$version"

    write-host "** Building Images (tag $tag) **"
    docker build -t snowjallen/homespeakerserver:$tag -f ./HomeSpeaker.Server/Dockerfile .
    docker build -t snowjallen/homespeakerwebclient:$tag -f ./HomeSpeaker.Client.Web/Dockerfile .

    write-host "** Pushing Images (tag $tag)**"
    docker push snowjallen/homespeakerserver:$tag
    docker push snowjallen/homespeakerwebclient:$tag
} else {
    $tag = "v$version"
}

write-host "** Copy latest restart.ps1 and docker-compose.yml files **"
scp scripts/restart.ps1 "pi@$($server):~/scripts/"
scp scripts/docker-compose.yml "pi@$($server):~/scripts/"

write-host "** Restarting docker-compose on rpi4  (tag $tag) **"
ssh pi@$server "cd scripts && pwsh ./restart.ps1 -tag $tag"