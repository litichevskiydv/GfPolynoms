namespace AppliedAlgebra.GfPolynoms.Tests.GaluaFields
{
    using System;
    using Extensions;
    using GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class FieldElementExtensionsTests
    {
        [UsedImplicitly]
        public static TheoryData<TransferFieldElementParametersValidationTestCase> TransferToSubfieldParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<TransferFieldElementParametersValidationTestCase> TransferFromSubfieldParametersValidationTestCases;

        static FieldElementExtensionsTests()
        {
            var gf3 = GaloisField.Create(3);
            var gf5 = GaloisField.Create(5);
            var gf9 = GaloisField.Create(9);
            var gf27 = GaloisField.Create(27);
            TransferToSubfieldParametersValidationTestCases
                = new TheoryData<TransferFieldElementParametersValidationTestCase>
                  {
                      new TransferFieldElementParametersValidationTestCase {NewField = gf9},
                      new TransferFieldElementParametersValidationTestCase {FieldElement = gf3.CreateElement(2)},
                      new TransferFieldElementParametersValidationTestCase {FieldElement = gf3.CreateElement(2), NewField = gf5},
                      new TransferFieldElementParametersValidationTestCase {FieldElement = gf9.CreateElement(7), NewField = gf27}
                  };
            TransferFromSubfieldParametersValidationTestCases
                = new TheoryData<TransferFieldElementParametersValidationTestCase>
                  {
                      new TransferFieldElementParametersValidationTestCase {NewField = gf3},
                      new TransferFieldElementParametersValidationTestCase {FieldElement = gf9.One()},
                      new TransferFieldElementParametersValidationTestCase {FieldElement = gf5.One(), NewField = gf3},
                      new TransferFieldElementParametersValidationTestCase {FieldElement = gf27.CreateElement(7), NewField = gf9},
                      new TransferFieldElementParametersValidationTestCase
                      {
                          FieldElement = gf9.CreateElement(gf9.PowGeneratingElement(5)),
                          NewField = gf3
                      }
                  };
        }

        [Theory]
        [MemberData(nameof(TransferToSubfieldParametersValidationTestCases))]
        public void TransferToSubfieldMustValidateParameters(TransferFieldElementParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FieldElement.TransferToSubfield(testCase.NewField));
        }

        [Theory]
        [MemberData(nameof(TransferFromSubfieldParametersValidationTestCases))]
        public void TransferFromSubfieldMustValidateParameters(TransferFieldElementParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FieldElement.TransferFromSubfield(testCase.NewField));
        }
    }
}