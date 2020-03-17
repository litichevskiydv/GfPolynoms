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
        [UsedImplicitly]
        public static readonly TheoryData<GenerateConjugacyClassesParametersValidationTestCase> GenerateConjugacyClassesParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<TransferElementParametersValidationTestCase> TransferToSubfieldParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<TransferElementParametersValidationTestCase> TransferFromSubfieldParametersValidationTestCases;

        static GaloisFieldExtensionsTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);
            var gf4 = GaloisField.Create(4);
            var gf5 = GaloisField.Create(5);
            var gf9 = GaloisField.Create(9);
            var gf27 = GaloisField.Create(27);
            FieldExtensionSearchParametersValidationTestCases
                = new TheoryData<FieldExtensionSearchParametersValidationTestCase>
                  {
                      new FieldExtensionSearchParametersValidationTestCase {RootOrder = 3},
                      new FieldExtensionSearchParametersValidationTestCase {Field = gf3, RootOrder = -1}
                  };
            PrimitiveRootObtainingParametersValidationTestCases
                = new TheoryData<PrimitiveRootObtainingParametersValidationTestCase>
                  {
                      new PrimitiveRootObtainingParametersValidationTestCase {RootOrder = 3},
                      new PrimitiveRootObtainingParametersValidationTestCase {Field = gf3, RootOrder = -1},
                      new PrimitiveRootObtainingParametersValidationTestCase {Field = gf4, RootOrder = 2}
                  };
            GenerateConjugacyClassesParametersValidationTestCases
                = new TheoryData<GenerateConjugacyClassesParametersValidationTestCase>
                  {
                      new GenerateConjugacyClassesParametersValidationTestCase {Modulus = 7},
                      new GenerateConjugacyClassesParametersValidationTestCase {Field = gf2, Modulus = -1}
                  };
            TransferToSubfieldParametersValidationTestCases
                = new TheoryData<TransferElementParametersValidationTestCase>
                  {
                      new TransferElementParametersValidationTestCase {FieldElement = 1, NewField = gf9},
                      new TransferElementParametersValidationTestCase {Field = gf3, FieldElement = 1},
                      new TransferElementParametersValidationTestCase {Field = gf3, FieldElement = 4, NewField = gf9},
                      new TransferElementParametersValidationTestCase {Field = gf3, FieldElement = 1, NewField = gf5},
                      new TransferElementParametersValidationTestCase {Field = gf9, FieldElement = 5, NewField = gf27}
                  };
            TransferFromSubfieldParametersValidationTestCases
                = new TheoryData<TransferElementParametersValidationTestCase>
                  {
                      new TransferElementParametersValidationTestCase {FieldElement = 1, NewField = gf3},
                      new TransferElementParametersValidationTestCase {Field = gf9, FieldElement = 1},
                      new TransferElementParametersValidationTestCase {Field = gf9, FieldElement = 11, NewField = gf3},
                      new TransferElementParametersValidationTestCase {Field = gf5, FieldElement = 1, NewField = gf3},
                      new TransferElementParametersValidationTestCase {Field = gf27, FieldElement = 5, NewField = gf9},
                      new TransferElementParametersValidationTestCase
                      {
                          Field = gf9, 
                          FieldElement = gf9.PowGeneratingElement(5), 
                          NewField = gf3
                      }
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
            Assert.Equal(1, fieldExtension.Pow(fieldExtension.PowGeneratingElement((fieldExtension.Order - 1) / rootOrder), rootOrder));
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

        [Theory]
        [MemberData(nameof(GenerateConjugacyClassesParametersValidationTestCases))]
        public void GenerateConjugacyClassesMustValidateParameters(GenerateConjugacyClassesParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Field.GenerateConjugacyClasses(testCase.Modulus));
        }

        [Fact]
        public void MustGenerateConjugacyClasses()
        {
            // Given
            var field = GaloisField.Create(2);
            const int modulus = 7;

            // When
            var actualConjugacyClasses = field.GenerateConjugacyClasses(modulus);

            // Then
            var expectedFirstConjugacyClasses = new[] {new[] {0}, new[] {1, 2, 4}, new[] {3, 6, 5}};
            Assert.Equal(expectedFirstConjugacyClasses, actualConjugacyClasses);
        }

        [Theory]
        [MemberData(nameof(TransferToSubfieldParametersValidationTestCases))]
        public void TransferToSubfieldMustValidateParameters(TransferElementParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Field.TransferElementToSubfield(testCase.FieldElement, testCase.NewField));
        }

        [Theory]
        [MemberData(nameof(TransferFromSubfieldParametersValidationTestCases))]
        public void TransferFromSubfieldMustValidateParameters(TransferElementParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Field.TransferElementFromSubfield(testCase.FieldElement, testCase.NewField));
        }
    }
}