(Get-Content src\GfPolynoms\GfPolynoms.csproj) -replace 'portable', 'full' | Set-Content src\GfPolynoms\GfPolynoms.csproj
(Get-Content src\GfAlgorithms\GfAlgorithms.csproj) -replace 'portable', 'full' | Set-Content src\GfAlgorithms\GfAlgorithms.csproj
(Get-Content src\RsCodesTools\RsCodesTools.csproj) -replace 'portable', 'full' | Set-Content src\RsCodesTools\RsCodesTools.csproj
(Get-Content src\WaveletCodesTools\WaveletCodesTools.csproj) -replace 'portable', 'full' | Set-Content src\WaveletCodesTools\WaveletCodesTools.csproj
