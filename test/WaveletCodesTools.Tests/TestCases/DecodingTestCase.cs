﻿namespace AppliedAlgebra.WaveletCodesTools.Tests.TestCases
{
    using CodesAbstractions;
    using GfPolynoms;

    public class DecodingTestCase
    {
        public ICode Code { get; set; }

        public FieldElement[] InformationWord { get; set; }

        public FieldElement[] AdditiveNoise { get; set; }
    }
}