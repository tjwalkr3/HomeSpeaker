# HomeSpeaker

## Deployment Notes

You have to create a certificate on the host machine

```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p $CREDENTIAL_PLACEHOLDER$
dotnet dev-certs https --trust
```

Then in the docker compose you can map a volume to that dir and set the password as an environment variable.