dotnet build ./webapp/webapp.csproj
if not %errorlevel% == 1 (npm run webpack --prefix ./webapp)