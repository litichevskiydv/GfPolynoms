(Get-Content src\AppliedAlgebra.GfPolynoms\project.json) -replace 'portable', 'full' | Set-Content src\AppliedAlgebra.GfPolynoms\project.json
(Get-Content src\AppliedAlgebra.GfAlgorithms\project.json) -replace 'portable', 'full' | Set-Content src\AppliedAlgebra.GfAlgorithms\project.json
(Get-Content src\AppliedAlgebra.RsCodesTools\project.json) -replace 'portable', 'full' | Set-Content src\AppliedAlgebra.RsCodesTools\project.json
(Get-Content src\AppliedAlgebra.WaveletCodesTools\project.json) -replace 'portable', 'full' | Set-Content src\AppliedAlgebra.WaveletCodesTools\project.json
