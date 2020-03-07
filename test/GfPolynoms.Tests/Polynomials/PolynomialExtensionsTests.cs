namespace AppliedAlgebra.GfPolynoms.Tests.Polynomials
{
    using System;
    using System.Linq;
    using Extensions;
    using GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class PolynomialExtensionsTests
    {
        [UsedImplicitly]
        public static TheoryData<ChangeFieldParametersValidationTestCase> ChangeFieldParametersValidationTestCases;

        static PolynomialExtensionsTests()
        {
            ChangeFieldParametersValidationTestCases
                = new TheoryData<ChangeFieldParametersValidationTestCase>
                  {
                      new ChangeFieldParametersValidationTestCase {NewField = GaloisField.Create(4)},
                      new ChangeFieldParametersValidationTestCase {Polynomial = new Polynomial(GaloisField.Create(2), 1, 0, 1)},
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(4), 3),
                          NewField = GaloisField.Create(3)
                      },
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(4), 3),
                          NewField = GaloisField.Create(2)
                      }
                  };
        }

        [Fact]
        public void ShouldGetPolynomialSpectrum()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var polynomial = new Polynomial(gf5, 1, 1, 1);

            // When
            var actualSpectrum = polynomial.GetSpectrum();

            // Then
            var expectedSpectrum = new[] { 3, 2, 1, 3 }.Select(x => gf5.CreateElement(x)).ToArray();
            Assert.Equal(expectedSpectrum, actualSpectrum);
        }

        [Theory]
        [MemberData(nameof(ChangeFieldParametersValidationTestCases))]
        public void ChangeFieldMustValidateParameters(ChangeFieldParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Polynomial.ChangeField(testCase.NewField));
        }

        [Fact]
        public void MustChangePolynomialField()
        {
            // Given
            var sourcePolynomial = new Polynomial(GaloisField.Create(2), 1, 0, 1);
            var newField = GaloisField.Create(4);

            // When
            var actualPolynomial = sourcePolynomial.ChangeField(newField);

            // Then
            var expectedPolynomial = new Polynomial(newField, 1, 0, 1);
            Assert.Equal(expectedPolynomial, actualPolynomial);
        }
    }
}