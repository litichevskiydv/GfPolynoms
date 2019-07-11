namespace AppliedAlgebra.RsCodesTools.Tests.TestCases
{
    using System;
    using GfPolynoms;

    public class ListDecoderTestCase
    {
        public int N { get; set; }

        public int K { get; set; }

        public Tuple<FieldElement, FieldElement>[] DecodedCodeword { get; set; }

        public int MinCorrectValuesCount { get; set; }

        public Polynomial Expected { get; set; }
    }
}