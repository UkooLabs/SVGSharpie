@echo off


cd tests\CodeCoverage

nuget restore packages.config -PackagesDirectory .

cd ..
cd ..

dotnet restore EquinoxLabs.SVGSharpie.sln
rem Clean the solution to force a rebuild with /p:codecov=true
dotnet clean EquinoxLabs.SVGSharpie.sln -c Release
rem The -threshold options prevents this taking ages...
tests\CodeCoverage\OpenCover.4.6.519\tools\OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"test tests\EquinoxLabs.SVGSharpie.ImageSharp.Tests\EquinoxLabs.SVGSharpie.ImageSharp.Tests.csproj -c Release -f netcoreapp2.1 /p:codecov=true" -register:user -threshold:10 -oldStyle -safemode:off -output:.\SVGSharpie.Coverage.xml -hideskipped:All -returntargetcode -filter:"+[EquinoxLabs.SVGSharpie*]*" 

if %errorlevel% neq 0 exit /b %errorlevel%

SET PATH=C:\python34;C:\python34\Scripts;%PATH%
python -m pip install --upgrade pip
pip install codecov
codecov -f "SVGSharpie.Coverage.xml"