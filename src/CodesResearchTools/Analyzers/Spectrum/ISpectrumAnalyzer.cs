namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public interface ISpectrumAnalyzer
    {
        IReadOnlyDictionary<int, BigInteger> Analyze(ICode code, SpectrumAnalyzerOptions options = null);

        IReadOnlyDictionary<int, BigInteger> Analyze(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            SpectrumAnalyzerOptions options = null
        );
    }
}