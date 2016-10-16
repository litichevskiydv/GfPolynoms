dotnet restore "src\AppliedAlgebra.GfPolynoms"
dotnet restore "src\AppliedAlgebra.GfAlgorithms"
dotnet restore "src\AppliedAlgebra.RsCodesTools"
dotnet restore "src\AppliedAlgebra.WaveletCodesTools"
dotnet restore "src\Utils\SpectrumAnalyzer"
dotnet restore "src\Utils\WaveletCodesListDecodingAnalyzer"
dotnet restore "test\GfPolynoms.Tests"
dotnet restore "test\GfAlgorithms.Tests"
dotnet restore "test\RsCodesTools.Tests"
dotnet restore "test\WaveletCodesTools.Tests"

dotnet build "src\AppliedAlgebra.GfPolynoms" -c %CONFIGURATION% --no-dependencies
dotnet build "src\AppliedAlgebra.GfAlgorithms" -c %CONFIGURATION% --no-dependencies
dotnet build "src\AppliedAlgebra.RsCodesTools" -c %CONFIGURATION% --no-dependencies
dotnet build "src\AppliedAlgebra.WaveletCodesTools" -c %CONFIGURATION% --no-dependencies
dotnet build "src\Utils\SpectrumAnalyzer" -c %CONFIGURATION% --no-dependencies
dotnet build "src\Utils\WaveletCodesListDecodingAnalyzer" -c %CONFIGURATION% --no-dependencies
dotnet build "test\GfPolynoms.Tests" -c %CONFIGURATION% --no-dependencies
dotnet build "test\GfAlgorithms.Tests" -c %CONFIGURATION% --no-dependencies
dotnet build "test\RsCodesTools.Tests" -c %CONFIGURATION% --no-dependencies
dotnet build "test\WaveletCodesTools.Tests" -c %CONFIGURATION% --no-dependencies