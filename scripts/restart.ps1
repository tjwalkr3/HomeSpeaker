# Restart script for the pantry website
param(
    [string]$tag
)

write-host "Got tag $tag from parameter"

#$sha = $tag.split(":") | where  -filterscript {$_ -like  "sha*" }
$sha = $tag
write-host "Setting 'BuildID' to $sha"
$ENV:BuildID=$sha

docker-compose pull
docker-compose up -d
