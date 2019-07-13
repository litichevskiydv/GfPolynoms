namespace AppliedAlgebra.CodesResearchTools.Tests.TestCases
{
    using System;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class CodeDistanceAnalyzerTestCase
    {
        public GaloisField Field { get; set; }

        public int InformationWordLength { get; set; }

        public Func<int[], FieldElement[]> EncodingProcedure { get; set; }

        public int Expected { get; set; }
    }
}