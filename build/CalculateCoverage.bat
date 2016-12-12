powershell.exe -file build\SetFullDebugType.ps1

nuget install OpenCover -ExcludeVersion -OutputDirectory tools
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\GfPolynoms.Tests\project.json"" -c %1" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -output:coverage.xml
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\GfAlgorithms.Tests\project.json"" -c %1" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\RsCodesTools.Tests\project.json"" -c %1" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\WaveletCodesTools.Tests\project.json"" -c %1" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml

SET PATH=C:\\Python34;C:\\Python34\\Scripts;%PATH%
pip install codecov
codecov -f "coverage.xml"