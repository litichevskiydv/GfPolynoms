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
        [UsedImplicitly]
        public static readonly TheoryData<PrimitiveRootObtainingParametersValidationTestCase> PrimitiveRootObtainingParametersValidationTestCases;

        static GaloisFieldExtensionsTests()
        {
            FieldExtensionSearchParametersValidationTestCases
                = new TheoryData<FieldExtensionSearchParametersValidationTestCase>
                  {
                      new FieldExtensionSearchParametersValidationTestCase {RootOrder = 3},
                      new FieldExtensionSearchParametersValidationTestCase {Field = GaloisField.Create(3), RootOrder = -1}
                  };
            PrimitiveRootObtainingParametersValidationTestCases
                = new TheoryData<PrimitiveRootObtainingParametersValidationTestCase>
                  {
                      new PrimitiveRootObtainingParametersValidationTestCase {RootOrder = 3},
                      new PrimitiveRootObtainingParametersValidationTestCase {Field = GaloisField.Create(3), RootOrder = -1},
                      new PrimitiveRootObtainingParametersValidationTestCase {Field = GaloisField.Create(4), RootOrder = 2}
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

        [Theory]
        [MemberData(nameof(PrimitiveRootObtainingParametersValidationTestCases))]
        public void MustValidatePrimitiveRootObtainingParameters(PrimitiveRootObtainingParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Field.GetPrimitiveRoot(testCase.RootOrder));
        }

        [Fact]
        public void MustObtainPrimitiveRoot()
        {
            // Given
            var field = GaloisField.Create(9);
            const int rootOrder = 4;

            // When
            var primitiveRoot = field.GetPrimitiveRoot(rootOrder);

            // Then
            Assert.Equal(field.One(), primitiveRoot.Pow(rootOrder));
        }
    }
}