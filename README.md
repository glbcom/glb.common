# Play.commmon

Common library used by Globalcom microservices ecosystem

## Create and publish package

```powershell
$version="1.0.1"
$owner="glb"
$gh_pat="[personal access token here]"

dotnet pack src\Play.Common --configuration Release -p:packageversion=$version -p:RepositoryUrl=https://github.com/$owner/play.common -o ..\..\nuget_packages

dotnet nuget push ..\..\nuget_packages\Play.Common.$version.nupkg --api-key $gh_pat --source "github"

```
