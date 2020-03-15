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
        public static TheoryData<ChangeFieldParametersValidationTestCase> TransferToSubfieldParametersValidationTestCases;

        static PolynomialExtensionsTests()
        {
            TransferToSubfieldParametersValidationTestCases
                = new TheoryData<ChangeFieldParametersValidationTestCase>
                  {
                      new ChangeFieldParametersValidationTestCase {NewField = GaloisField.Create(3)},
                      new ChangeFieldParametersValidationTestCase {Polynomial = new Polynomial(GaloisField.Create(3), 1)},
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(9), 1, 0, 2),
                          NewField = GaloisField.Create(27)
                      },
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(3), 1, 0, 2),
                          NewField = GaloisField.Create(5)
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
        [MemberData(nameof(TransferToSubfieldParametersValidationTestCases))]
        public void TransferToSubfieldMustValidateParameters(ChangeFieldParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Polynomial.TransferToSubfield(testCase.NewField));
        }
    }
}