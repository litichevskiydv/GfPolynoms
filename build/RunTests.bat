dotnet test "test\AppliedAlgebra.GfPolynoms.Tests" -c %CONFIGURATION% -f netcoreapp1.0
dotnet test "test\AppliedAlgebra.GfAlgorithms.Tests" -c %CONFIGURATION% -f netcoreapp1.0
dotnet test "test\AppliedAlgebra.RsCodesTools.Tests" -c %CONFIGURATION% -f netcoreapp1.0
dotnet test "test\AppliedAlgebra.WaveletCodesTools.Tests" -c %CONFIGURATION% -f netcoreapp1.0