namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.BiVariablePolynomial
{
    using BiVariablePolynomials;
    using GfPolynoms;

    public class PartialEvaluationTestCase
    {
        public BiVariablePolynomial Polynomial { get; set; }

        public FieldElement VariableValue { get; set; }

        public Polynomial Expected { get; set; }
    }
}