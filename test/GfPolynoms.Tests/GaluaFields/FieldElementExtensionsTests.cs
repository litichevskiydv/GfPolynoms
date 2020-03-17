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
        }

        [Theory]
        [MemberData(nameof(TransferToSubfieldParametersValidationTestCases))]
        public void TransferToSubfieldMustValidateParameters(TransferFieldElementParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FieldElement.TransferToSubfield(testCase.NewField));
        }
    }
}