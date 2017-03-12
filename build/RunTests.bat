dotnet test "test\GfPolynoms.Tests\GfPolynoms.Tests.csproj" -c %1 --no-build
if %errorlevel% neq 0 exit /b %errorlevel% 
dotnet test "test\GfAlgorithms.Tests\GfAlgorithms.Tests.csproj" -c %1 --no-build
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet test "test\RsCodesTools.Tests\RsCodesTools.Tests.csproj" -c %1 --no-build
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet test "test\WaveletCodesTools.Tests\WaveletCodesTools.Tests.csproj" -c %1 --no-build
if %errorlevel% neq 0 exit /b %errorlevel%