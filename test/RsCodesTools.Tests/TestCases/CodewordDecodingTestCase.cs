namespace AppliedAlgebra.RsCodesTools.Tests.TestCases
{
    using CodesAbstractions;
    using GfPolynoms;

    public class CodewordDecodingTestCase
    {
        public ICode Code { get; set; }

        public FieldElement[] InformationWord { get; set; }

        public FieldElement[] AdditiveNoise { get; set; }
    }
}