dotnet restore "src\GfPolynoms" --no-dependencies
dotnet restore "src\GfAlgorithms" --no-dependencies
dotnet restore "src\RsCodesTools" --no-dependencies
dotnet restore "src\WaveletCodesTools" --no-dependencies
dotnet restore "src\Utils\SpectrumAnalyzer" --no-dependencies
dotnet restore "src\Utils\WaveletCodesListDecodingAnalyzer" --no-dependencies
dotnet restore "test\GfPolynoms.Tests" --no-dependencies
dotnet restore "test\GfAlgorithms.Tests" --no-dependencies
dotnet restore "test\RsCodesTools.Tests" --no-dependencies
dotnet restore "test\WaveletCodesTools.Tests" --no-dependencies

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