Add-Migration InitialCreate
Update-Database
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations add AddDateOfBirthToUser
dotnet ef database update
