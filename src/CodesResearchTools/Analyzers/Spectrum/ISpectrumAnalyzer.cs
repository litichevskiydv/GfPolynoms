namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;
    using System.Collections.Generic;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public interface ISpectrumAnalyzer
    {
        IReadOnlyDictionary<int, long> Analyze(ICode code, SpectrumAnalyzerOptions options = null);

        IReadOnlyDictionary<int, long> Analyze(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            SpectrumAnalyzerOptions options = null
        );
    }
}