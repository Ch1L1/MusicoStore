dotnet nuget locals all --clear

Remove-Item -Recurse -Force src\MusicoStore.DataAccessLayer\Migrations\*.cs 

dotnet ef migrations add Initial --project src\MusicoStore.DataAccessLayer --startup-project src\MusicoStore.WebApi

dotnet ef database drop --project src\MusicoStore.DataAccessLayer --startup-project src\MusicoStore.WebApi --force

dotnet ef database update --project src\MusicoStore.DataAccessLayer --startup-project src\MusicoStore.WebApi
