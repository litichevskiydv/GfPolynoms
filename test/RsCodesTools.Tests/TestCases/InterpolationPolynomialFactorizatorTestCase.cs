namespace AppliedAlgebra.RsCodesTools.Tests.TestCases
{
    using GfAlgorithms.BiVariablePolynomials;
    using GfPolynoms;

    public class InterpolationPolynomialFactorizatorTestCase
    {
        public BiVariablePolynomial Polynomial { get; set; }

        public int MaxFactorDegree { get; set; }

        public Polynomial[] Expected { get; set; }
    }
}