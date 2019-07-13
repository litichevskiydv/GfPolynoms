namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.BiVariablePolynomial
{
    using BiVariablePolynomials;
    using GfPolynoms;

    public class MultiplicationByFieldElementTestCase
    {
        public BiVariablePolynomial Polynomial { get; set; }

        public FieldElement Multiplier { get; set; }

        public BiVariablePolynomial Expected { get; set; }
    }
}