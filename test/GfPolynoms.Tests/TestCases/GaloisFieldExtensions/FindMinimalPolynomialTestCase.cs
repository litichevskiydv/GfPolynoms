namespace AppliedAlgebra.GfPolynoms.Tests.TestCases.GaloisFieldExtensions
{
    using GaloisFields;

    public class FindMinimalPolynomialTestCase
    {
        public GaloisField FieldExtension { get; set; }

        public int FieldElement { get; set; }

        public Polynomial Expected { get; set; }
    }
}