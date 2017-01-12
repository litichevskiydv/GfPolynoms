dotnet test "test\GfPolynoms.Tests" -c %1
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet test "test\GfAlgorithms.Tests" -c %1
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet test "test\RsCodesTools.Tests" -c %1
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet test "test\WaveletCodesTools.Tests" -c %1
if %errorlevel% neq 0 exit /b %errorlevel%