namespace AppliedAlgebra.CodesAbstractions.Tests
{
    using System;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class BasicCodewordMutatorTests
    {
        public class MutateParametersValidationTestCase
        {
            public FieldElement[] OriginalCodeword { get; set; }

            public int RequiredLength { get; set; }
        }

        public class CodewordMutationTestCase : MutateParametersValidationTestCase
        {
            public FieldElement[] Expected { get; set; }
        }

        private readonly BasicCodewordMutator _codewordMutator;

        [UsedImplicitly]
        public static TheoryData<MutateParametersValidationTestCase> MutateParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<CodewordMutationTestCase> CodewordMutationTestCases;

        static BasicCodewordMutatorTests()
        {
            var gf3 = GaloisField.Create(3);
            var originalCodeword = gf3.CreateElementsVector(0, 1, 2, 1);

            MutateParametersValidationTestCases
                = new TheoryData<MutateParametersValidationTestCase>
                  {
                      new MutateParametersValidationTestCase(),
                      new MutateParametersValidationTestCase {OriginalCodeword = new FieldElement[0]},
                      new MutateParametersValidationTestCase{OriginalCodeword = originalCodeword, RequiredLength = -1}
                  };
            CodewordMutationTestCases
                = new TheoryData<CodewordMutationTestCase>
                  {
                      new CodewordMutationTestCase
                      {
                          OriginalCodeword = originalCodeword,
                          RequiredLength = originalCodeword.Length + 1,
                          Expected = gf3.CreateElementsVector(0, 1, 2, 1, 1)
                      },
                      new CodewordMutationTestCase
                      {
                          OriginalCodeword = originalCodeword,
                          RequiredLength = originalCodeword.Length,
                          Expected = originalCodeword
                      },
                      new CodewordMutationTestCase
                      {
                          OriginalCodeword = originalCodeword,
                          RequiredLength = originalCodeword.Length -1,
                          Expected = gf3.CreateElementsVector(0, 1, 2)
                      }
                  };
        }

        public BasicCodewordMutatorTests()
        {
            _codewordMutator = new BasicCodewordMutator();
        }

        [Theory]
        [MemberData(nameof(MutateParametersValidationTestCases))]
        public void MutateMustValidateParameters(MutateParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _codewordMutator.Mutate(testCase.OriginalCodeword, testCase.RequiredLength));
        }

        [Theory]
        [MemberData(nameof(CodewordMutationTestCases))]
        public void MustMutateCodeword(CodewordMutationTestCase testCase)
        {
            // When
            var actual = _codewordMutator.Mutate(testCase.OriginalCodeword, testCase.RequiredLength);

            // Then
            Assert.Equal(testCase.Expected, actual);
        }

        [Fact]
        public void CanNotAchieveRequiredLength()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var originalCodeWord = gf3.CreateElementsVector(1, 2, 1, 0);
            var requiredLength = originalCodeWord.Length + 2;

            // When, Then
            Assert.Throws<InvalidOperationException>(() => _codewordMutator.Mutate(originalCodeWord, requiredLength));
        }
    }
}