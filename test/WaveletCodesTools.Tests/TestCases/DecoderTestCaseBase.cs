namespace AppliedAlgebra.WaveletCodesTools.Tests.TestCases
{
    using System;
    using GfPolynoms;

    public class DecoderTestCaseBase
    {
        public int N { get; set; }

        public int K { get; set; }

        public int D { get; set; }

        public Polynomial GeneratingPolynomial { get; set; }

        public Tuple<FieldElement, FieldElement>[] DecodedCodeword { get; set; }

        public int MinCorrectValuesCount { get; set; }

        public Polynomial Expected { get; set; }
    }
}