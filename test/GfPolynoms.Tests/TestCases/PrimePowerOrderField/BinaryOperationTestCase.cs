namespace AppliedAlgebra.GfPolynoms.Tests.TestCases.PrimePowerOrderField
{
    using GaloisFields;

    public class BinaryOperationTestCase
    {
        public GaloisField Field { get; set; }

        public int FirstOperand { get; set; }

        public int SecondOperand { get; set; }

        public int Expected { get; set; }
    }
}