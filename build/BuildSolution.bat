dotnet restore "src\GfPolynoms"
dotnet restore "src\GfAlgorithms"
dotnet restore "src\RsCodesTools"
dotnet restore "src\WaveletCodesTools"
dotnet restore "src\Utils\SpectrumAnalyzer"
dotnet restore "src\Utils\WaveletCodesListDecodingAnalyzer"
dotnet restore "test\GfPolynoms.Tests"
dotnet restore "test\GfAlgorithms.Tests"
dotnet restore "test\RsCodesTools.Tests"
dotnet restore "test\WaveletCodesTools.Tests"

dotnet build "src\GfPolynoms" -c %1 --no-dependencies
dotnet build "src\GfAlgorithms" -c %1 --no-dependencies
dotnet build "src\RsCodesTools" -c %1 --no-dependencies
dotnet build "src\WaveletCodesTools" -c %1 --no-dependencies
dotnet build "src\Utils\SpectrumAnalyzer" -c %1 --no-dependencies
dotnet build "src\Utils\WaveletCodesListDecodingAnalyzer" -c %1 --no-dependencies
dotnet build "test\GfPolynoms.Tests" -c %1 --no-dependencies
dotnet build "test\GfAlgorithms.Tests" -c %1 --no-dependencies
dotnet build "test\RsCodesTools.Tests" -c %1 --no-dependencies
dotnet build "test\WaveletCodesTools.Tests" -c %1 --no-dependencies