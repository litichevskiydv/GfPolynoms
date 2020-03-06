namespace AppliedAlgebra.GfPolynoms.Tests.GaluaFields
{
    using System;
    using Extensions;
    using GaloisFields;
    using JetBrains.Annotations;
    using TestCases.GaloisFieldExtensions;
    using Xunit;

    public class GaloisFieldExtensionsTests
    {
        [UsedImplicitly]
        public static readonly TheoryData<FieldExtensionSearchParametersValidationTestCase> FieldExtensionSearchParametersValidationTestCases;

        static GaloisFieldExtensionsTests()
        {
            FieldExtensionSearchParametersValidationTestCases
                = new TheoryData<FieldExtensionSearchParametersValidationTestCase>
                  {
                      new FieldExtensionSearchParametersValidationTestCase {RootOrder = 3},
                      new FieldExtensionSearchParametersValidationTestCase {Field = GaloisField.Create(3), RootOrder = -1}
                  };
        }

        [Theory]
        [MemberData(nameof(FieldExtensionSearchParametersValidationTestCases))]
        public void MustValidateExtensionSearchParameters(FieldExtensionSearchParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Field.FindExtensionContainingPrimitiveRoot(testCase.RootOrder));
        }

        [Fact]
        public void MustFindFieldExtensionContainingPrimitiveRoot()
        {
            // Given
            var field = GaloisField.Create(4);
            const int rootOrder = 9;

            // When
            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(rootOrder);

            // Then
            Assert.Equal(1, fieldExtension.Pow(fieldExtension.GetGeneratingElementPower((fieldExtension.Order - 1) / rootOrder), rootOrder));
        }
    }
}