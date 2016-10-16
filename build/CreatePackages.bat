dotnet pack "src\AppliedAlgebra.GfPolynoms" -c %CONFIGURATION% --version-suffix beta1-build%APPVEYOR_BUILD_NUMBER%
dotnet pack "src\AppliedAlgebra.GfAlgorithms" -c %CONFIGURATION% --version-suffix beta1-build%APPVEYOR_BUILD_NUMBER%
dotnet pack "src\AppliedAlgebra.RsCodesTools" -c %CONFIGURATION% --version-suffix beta1-build%APPVEYOR_BUILD_NUMBER%
dotnet pack "src\AppliedAlgebra.WaveletCodesTools" -c %CONFIGURATION% --version-suffix beta1-build%APPVEYOR_BUILD_NUMBER%