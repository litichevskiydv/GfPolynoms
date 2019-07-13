namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.BiVariablePolynomial
{
    using BiVariablePolynomials;
    using GfPolynoms;

    public class HasseDerivativeCalculationTestCase
    {
        public BiVariablePolynomial Polynomial { get; set; }

        public int R { get; set; }

        public int S { get; set; }

        public FieldElement XValue { get; set; }

        public FieldElement YValue { get; set; }

        public FieldElement Expected { get; set; }
    }
}