namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.BiVariablePolynomial
{
    using BiVariablePolynomials;
    using GfPolynoms;

    public class EvaluationTestCase
    {
        public BiVariablePolynomial Polynomial { get; set; }

        public FieldElement XValue { get; set; }

        public FieldElement YValue { get; set; }

        public FieldElement Expected { get; set; }
    }
}