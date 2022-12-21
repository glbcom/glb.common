# Glb.common

Common library used by Globalcom microservices ecosystem

## Create and publish package

```powershell
$version="1.0.1"
$owner="glb"
$gh_pat="PAT HERE"

dotnet pack src\Glb.Common --configuration Release -p:packageversion=$version -p:RepositoryUrl=https://github.com/glbcom/glb.common -o ..\..\nuget_packages

dotnet nuget push ..\..\nuget_packages\Glb.Common.$version.nupkg --api-key $gh_pat --source "github"

```
