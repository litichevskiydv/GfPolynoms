namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsFactorizer;
    using PolynomialsGcdFinder;
    using Xunit;

    public class BerlekampFactorizerTests
    {
        private readonly BerlekampFactorizer _factorizer;

        [UsedImplicitly] 
        public static readonly TheoryData<Polynomial> FactorizeTestsData;

        private static Polynomial PreparePolynomialForSearchOfAllIrreducibleFactorsOfGivenDegree(GaloisField field, int requiredDegree)
        {
            var one = new Polynomial(field.One());
            return (one >> (int) Math.Pow(field.Order, requiredDegree)) + (one >> 1);
        }

        static BerlekampFactorizerTests()
        {
            var gf3 = GaloisField.Create(3);
            var gf2 = GaloisField.Create(2);

            FactorizeTestsData
                = new TheoryData<Polynomial>
                  {
                      new Polynomial(gf3, 2, 2),
                      PreparePolynomialForSearchOfAllIrreducibleFactorsOfGivenDegree(gf3, 2),
                      PreparePolynomialForSearchOfAllIrreducibleFactorsOfGivenDegree(gf2, 4),
                      PreparePolynomialForSearchOfAllIrreducibleFactorsOfGivenDegree(gf2, 8),
                      PreparePolynomialForSearchOfAllIrreducibleFactorsOfGivenDegree(gf2, 9)
                  };
        }

        public BerlekampFactorizerTests()
        {
            _factorizer = new BerlekampFactorizer(new RecursiveGcdFinder());
        }

        [Fact]
        public void FactorizeMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => _factorizer.Factorize(null).ToArray());
        }

        [Theory]
        [MemberData(nameof(FactorizeTestsData))]
        public void MustFactorPolynomial(Polynomial expectedPolynomial)
        {
            // When
            var factors = _factorizer.Factorize(expectedPolynomial);

            // Then
            var actualPolynomial = new Polynomial(expectedPolynomial.Field.One());
            foreach (var (factor, degree) in factors)
            {
                if(factor.Degree > 1)
                    for (var fieldElement = 0; fieldElement < expectedPolynomial.Field.Order; fieldElement++)
                        Assert.NotEqual(0, factor.Evaluate(fieldElement));

                for (var i = 0; i < degree; i++)
                    actualPolynomial *= factor;
            }
            Assert.Equal(expectedPolynomial, actualPolynomial);
        }
    }
}