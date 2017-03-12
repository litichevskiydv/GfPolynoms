powershell.exe -file build\SetFullDebugType.ps1
nuget install OpenCover -ExcludeVersion -OutputDirectory tools

tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\GfPolynoms.Tests\GfPolynoms.Tests.csproj"" -c Debug" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -output:coverage.xml
if %errorlevel% neq 0 exit /b %errorlevel%
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\GfAlgorithms.Tests\GfAlgorithms.Tests.csproj"" -c Debug" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
if %errorlevel% neq 0 exit /b %errorlevel%
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\RsCodesTools.Tests\RsCodesTools.Tests.csproj"" -c Debug" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
if %errorlevel% neq 0 exit /b %errorlevel%
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\WaveletCodesTools.Tests\WaveletCodesTools.Tests.csproj"" -c Debug" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
if %errorlevel% neq 0 exit /b %errorlevel%

SET PATH=C:\\Python34;C:\\Python34\\Scripts;%PATH%
pip install codecov
codecov -f "coverage.xml"