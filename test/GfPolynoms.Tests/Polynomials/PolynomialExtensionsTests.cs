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
        [UsedImplicitly]
        public static TheoryData<ChangeFieldParametersValidationTestCase> TransferFromSubfieldParametersValidationTestCases;

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
            TransferFromSubfieldParametersValidationTestCases
                = new TheoryData<ChangeFieldParametersValidationTestCase>
                  {
                      new ChangeFieldParametersValidationTestCase {NewField = GaloisField.Create(3)},
                      new ChangeFieldParametersValidationTestCase {Polynomial = new Polynomial(GaloisField.Create(9), 1)},
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(27), 1, 0, 2),
                          NewField = GaloisField.Create(9)
                      },
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(5), 1, 0, 2),
                          NewField = GaloisField.Create(3)
                      },
                      new ChangeFieldParametersValidationTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(9), 1, 7),
                          NewField = GaloisField.Create(3)
                      },
                  };
        }


        [Fact]
        public void MustGetPolynomialSpectrum()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var polynomial = new Polynomial(gf5, 1, 1, 1);

            // When
            var actualSpectrum = polynomial.GetSpectrum(3);

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

        [Theory]
        [MemberData(nameof(TransferFromSubfieldParametersValidationTestCases))]
        public void TransferFromSubfieldMustValidateParameters(ChangeFieldParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Polynomial.TransferFromSubfield(testCase.NewField));
        }

        [Fact]
        public void MustTransferPolynomialFromSubfield()
        {
            // Given
            var gf9 = GaloisField.Create(9);
            var gf81 = GaloisField.Create(81);
            var polynomial = new Polynomial(gf81, 1, 0, gf81.PowGeneratingElement(20), gf81.PowGeneratingElement(40), 1);

            // When
            var actualPolynomial = polynomial.TransferFromSubfield(gf9);

            // Then
            var expectedPolynomial = new Polynomial(gf9, 1, 0, gf9.PowGeneratingElement(2), gf9.PowGeneratingElement(4), 1);
            Assert.Equal(expectedPolynomial, actualPolynomial);
        }
    }
}