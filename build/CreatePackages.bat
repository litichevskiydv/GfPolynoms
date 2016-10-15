dotnet pack "src\GfPolynoms" -c %CONFIGURATION% --version-suffix beta%APPVEYOR_BUILD_NUMBER%
dotnet pack "src\GfAlgorithms" -c %CONFIGURATION% --version-suffix beta%APPVEYOR_BUILD_NUMBER%
dotnet pack "src\RsCodesTools" -c %CONFIGURATION% --version-suffix beta%APPVEYOR_BUILD_NUMBER%
dotnet pack "src\WaveletCodesTools" -c %CONFIGURATION% --version-suffix beta%APPVEYOR_BUILD_NUMBER%