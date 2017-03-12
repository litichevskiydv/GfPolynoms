dotnet pack "src\GfPolynoms" -c %1 --include-symbols --version-suffix %2
dotnet pack "src\GfAlgorithms" -c %1 --include-symbols --version-suffix %2
dotnet pack "src\RsCodesTools" -c %1 --include-symbols --version-suffix %2
dotnet pack "src\WaveletCodesTools" -c %1 --include-symbols --version-suffix %2