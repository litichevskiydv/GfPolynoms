namespace AppliedAlgebra.WaveletCodesTools.Tests.TestCases
{
    using GfPolynoms;

    public class EncodeParametersValidationTestCase
    {
        public int CodewordLength { get; set; }

        public int LevelsCount { get; set; }

        public (FieldElement[] h, FieldElement[] g) SynthesisFilters { get; set; }

        public FieldElement[] InformationWord { get; set; }
    }
}