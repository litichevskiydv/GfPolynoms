namespace AppliedAlgebra.CodesResearchTools.Tests.TestCases
{
    using System;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class AnalyzeSpectrumParametersValidationTestCase
    {
        public GaloisField Field { get; set; }

        public int InformationWordLength { get; set; }

        public Func<FieldElement[], FieldElement[]> EncodingProcedure { get; set; }
    }
}