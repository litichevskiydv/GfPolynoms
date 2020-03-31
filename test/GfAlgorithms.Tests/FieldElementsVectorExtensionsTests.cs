namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class FieldElementsVectorExtensionsTests
    {
        [UsedImplicitly]
        public static readonly TheoryData<FieldElement[]> GetSpectrumParametersValidationTestCases;

        static FieldElementsVectorExtensionsTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);

            GetSpectrumParametersValidationTestCases
                = new TheoryData<FieldElement[]>
                  {
                      null,
                      new FieldElement[0],
                      new[] {gf2.One(), null},
                      new[] {gf2.One(), gf3.One()}
                  };
        }

        [Theory]
        [MemberData(nameof(GetSpectrumParametersValidationTestCases))]
        public void GetSpectrumMustValidateParameters(FieldElement[] vector)
        {
            Assert.ThrowsAny<ArgumentException>(vector.GetSpectrum);
        }
    }
}