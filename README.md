# Glb.common

Common library used by Globalcom microservices ecosystem

## Push Code to repository

For GitHub:
```powershell

git remote set-url origin https://github.com/glbcom/glb.common.git
```

For GitLab: 
```powershell
git remote set-url origin https://gitlab.idm.net.lb/globalcom/glb-common.git
```

## Create and publish package on Github

```powershell
$version="1.1.83" 
$owner="glbcom"
$gh_pat="PAT HERE"

dotnet pack src\Glb.Common --configuration Release -p:packageversion=$version -p:RepositoryUrl=https://github.com/glbcom/glb.common -o ..\..\nuget_packages

dotnet nuget push ..\..\nuget_packages\Glb.Common.$version.nupkg --api-key $gh_pat --source "glbgithub"

```

## Create and publish the NuGet package on Gitlab

```powershell
$version="1.1.82"
$owner="globalcom"
$gh_pat="[PAT HERE]"

#this command is used to add a new nuget repositry:

dotnet nuget add source https://gitlab.idm.net.lb/api/v4/projects/33/packages/nuget/index.json -n glbCommon -u JenkinsCI -p $gh_pat

dotnet pack src\Glb.Common\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://gitlab.idm.net.lb/$owner/Glb.Common -o ..\..\nuget_packages

dotnet nuget push ..\..\nuget_packages\Glb.Common.$version.nupkg --api-key $gh_pat --source "glbCommon"

```