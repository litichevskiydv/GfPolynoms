dotnet restore "src\AppliedAlgebra.GfPolynoms"
dotnet restore "src\AppliedAlgebra.GfAlgorithms"
dotnet restore "src\AppliedAlgebra.RsCodesTools"
dotnet restore "src\AppliedAlgebra.WaveletCodesTools"
dotnet restore "src\Utils\AppliedAlgebra.SpectrumAnalyzer"
dotnet restore "src\Utils\AppliedAlgebra.WaveletCodesListDecodingAnalyzer"
dotnet restore "test\AppliedAlgebra.GfPolynoms.Tests"
dotnet restore "test\AppliedAlgebra.GfAlgorithms.Tests"
dotnet restore "test\AppliedAlgebra.RsCodesTools.Tests"
dotnet restore "test\AppliedAlgebra.WaveletCodesTools.Tests"

dotnet build "src\AppliedAlgebra.GfPolynoms" -c %CONFIGURATION% --no-dependencies
dotnet build "src\AppliedAlgebra.GfAlgorithms" -c %CONFIGURATION% --no-dependencies
dotnet build "src\AppliedAlgebra.RsCodesTools" -c %CONFIGURATION% --no-dependencies
dotnet build "src\AppliedAlgebra.WaveletCodesTools" -c %CONFIGURATION% --no-dependencies
dotnet build "src\Utils\AppliedAlgebra.SpectrumAnalyzer" -c %CONFIGURATION% --no-dependencies
dotnet build "src\Utils\AppliedAlgebra.WaveletCodesListDecodingAnalyzer" -c %CONFIGURATION% --no-dependencies
dotnet build "test\AppliedAlgebra.GfPolynoms.Tests" -c %CONFIGURATION% --no-dependencies
dotnet build "test\AppliedAlgebra.GfAlgorithms.Tests" -c %CONFIGURATION% --no-dependencies
dotnet build "test\AppliedAlgebra.RsCodesTools.Tests" -c %CONFIGURATION% --no-dependencies
dotnet build "test\AppliedAlgebra.WaveletCodesTools.Tests" -c %CONFIGURATION% --no-dependencies