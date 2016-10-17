dotnet pack "src\AppliedAlgebra.GfPolynoms" -c %1 --version-suffix %2
dotnet pack "src\AppliedAlgebra.GfAlgorithms" -c %1 --version-suffix %2
dotnet pack "src\AppliedAlgebra.RsCodesTools" -c %1 --version-suffix %2
dotnet pack "src\AppliedAlgebra.WaveletCodesTools" -c %1 --version-suffix %2