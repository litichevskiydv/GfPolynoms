namespace AppliedAlgebra.WaveletCodesTools.Tests.TestCases
{
    using GfPolynoms;

    public class DecoderTestCaseBase
    {
        public int N { get; set; }

        public int K { get; set; }

        public int D { get; set; }

        public Polynomial GeneratingPolynomial { get; set; }

        public (FieldElement xValue, FieldElement yValue)[] DecodedCodeword { get; set; }

        public int MinCorrectValuesCount { get; set; }

        public Polynomial Expected { get; set; }
    }
}