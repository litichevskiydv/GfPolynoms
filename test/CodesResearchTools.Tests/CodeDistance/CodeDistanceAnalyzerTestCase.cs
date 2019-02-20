namespace AppliedAlgebra.CodesResearchTools.Tests.CodeDistance
{
    using System;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class CodeDistanceAnalyzerTestCase
    {
        public GaloisField Field { get; set; }
        public int InformationWordLength { get; set; }
        public Func<FieldElement[], FieldElement[]> EncodingProcedure;
        public int ExpectedCodeDistance { get; set; }
    }
}