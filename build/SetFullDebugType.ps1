(Get-Content src\GfPolynoms\project.json) -replace 'portable', 'full' | Set-Content src\GfPolynoms\project.json
(Get-Content src\GfAlgorithms\project.json) -replace 'portable', 'full' | Set-Content src\GfAlgorithms\project.json
(Get-Content src\RsCodesTools\project.json) -replace 'portable', 'full' | Set-Content src\RsCodesTools\project.json
(Get-Content src\WaveletCodesTools\project.json) -replace 'portable', 'full' | Set-Content src\WaveletCodesTools\project.json
