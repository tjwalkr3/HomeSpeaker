# HomeSpeaker

## Deployment Notes

You have to create a certificate on the host machine

```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p $CREDENTIAL_PLACEHOLDER$
dotnet dev-certs https --trust
```

Then in the docker compose you can map a volume to that dir and set the password as an environment variable.

## Deploying

To have GitHub Actions deploy a new version, create a new tag

```bash
git tag -a yyyy.m.d -m yyyy.m.d
```

Then push those tags

```bash
git push --tags
```  

Then a new version will be deployed on the self-hosted runner.
