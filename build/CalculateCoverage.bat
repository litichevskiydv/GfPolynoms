nuget install OpenCover -ExcludeVersion -OutputDirectory tools
powershell.exe -file build\SetFullDebugType.ps1
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\AppliedAlgebra.GfPolynoms.Tests\project.json"" -c Debug -f net46" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -output:coverage.xml
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\AppliedAlgebra.GfAlgorithms.Tests\project.json"" -c Debug -f net46" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\AppliedAlgebra.RsCodesTools.Tests\project.json"" -c Debug -f net46" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
tools\OpenCover\tools\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:" test ""test\AppliedAlgebra.WaveletCodesTools.Tests\project.json"" -c Debug -f net46" -register:user -filter:"+[*]* -[xunit*]*" -threshold:100 -oldstyle -returntargetcode -mergeoutput -output:coverage.xml
SET PATH=C:\\Python34;C:\\Python34\\Scripts;%PATH%
pip install codecov
codecov -f "coverage.xml"