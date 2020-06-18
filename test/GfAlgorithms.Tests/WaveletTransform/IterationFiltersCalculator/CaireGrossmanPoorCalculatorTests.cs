namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using System;
    using System.Linq;
    using Extensions;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Matrices;
    using TestCases.WaveletTransform;
    using Xunit;


    public class CaireGrossmanPoorCalculatorTests
    {
        private readonly CaireGrossmanPoorCalculator _iterationFiltersCalculator;

        [UsedImplicitly]
        public static TheoryData<GetIterationFilterVectorParametersValidationTestCase> GetIterationFilterVectorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase> OrthogonalIterationFiltersVectorsCalculationTestCases;

        static CaireGrossmanPoorCalculatorTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);
            var gf17 = GaloisField.Create(17);

            var hSource1 = new[] {16, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}.Select(x => gf17.CreateElement(x)).ToArray();
            var gSource1 = new[] {1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}.Select(x => gf17.CreateElement(x)).ToArray();
            var hSource2 = new[] {1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1}.Select(x => gf2.CreateElement(x)).ToArray();
            var gSource2 = new[] {1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1}.Select(x => gf2.CreateElement(x)).ToArray();
            var hSource3 = new[] {2, 1, 0, 0, 1, 1, 0, 0}.Select(x => gf3.CreateElement(x)).ToArray();
            var gSource3 = new[] {0, 0, 1, 1, 0, 0, 1, 2}.Select(x => gf3.CreateElement(x)).ToArray();

            GetIterationFilterVectorParametersValidationTestCases
                = new TheoryData<GetIterationFilterVectorParametersValidationTestCase>
                  {
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = -1, SourceFilter = hSource3},
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = 1},
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = 1, SourceFilter = new FieldElement[0]},
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = 4, SourceFilter = hSource3}
                  };

            OrthogonalIterationFiltersVectorsCalculationTestCases
                = new TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase>
                  {
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(3, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(4, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource2, gSource2),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource3, gSource3),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource3, gSource3),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(3, hSource3, gSource3),
                  };
        }

        public CaireGrossmanPoorCalculatorTests()
        {
            _iterationFiltersCalculator = new CaireGrossmanPoorCalculator();
        }

        [Theory]
        [MemberData(nameof(GetIterationFilterVectorParametersValidationTestCases))]
        public void GetIterationFilterVectorMustValidateParameters(GetIterationFilterVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilter));
        }

        [Fact]
        public void GetIterationFilterPolynomialMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => _iterationFiltersCalculator.GetIterationFilter(1, (Polynomial)null));
        }

        private static void CheckIterationFiltersVectors(FieldElement[] iterationFilterH, FieldElement[] iterationFilterG, FieldElement multiplier = null)
        {
            var hMatrix = FieldElementsMatrix.DoubleCirculantMatrix(iterationFilterH);
            var hMatrixTransposed = FieldElementsMatrix.Transpose(hMatrix);
            var gMatrix = FieldElementsMatrix.DoubleCirculantMatrix(iterationFilterG);
            var gMatrixTransposed = FieldElementsMatrix.Transpose(gMatrix);
            var checkedMultiplier = multiplier ?? hMatrix.Field.One();

            var fullSizeIdentityMatrix = FieldElementsMatrix.IdentityMatrix(hMatrix.Field, iterationFilterH.Length);
            Assert.Equal(checkedMultiplier * fullSizeIdentityMatrix, hMatrixTransposed * hMatrix + gMatrixTransposed * gMatrix);

            var halfSizeIdentityMatrix = FieldElementsMatrix.IdentityMatrix(hMatrix.Field, iterationFilterH.Length / 2);
            var halfSizeZeroMatrix = FieldElementsMatrix.ZeroMatrix(hMatrix.Field, iterationFilterH.Length / 2);
            Assert.Equal(checkedMultiplier * halfSizeIdentityMatrix, hMatrix * hMatrixTransposed);
            Assert.Equal(checkedMultiplier * halfSizeIdentityMatrix, gMatrix * gMatrixTransposed);
            Assert.Equal(halfSizeZeroMatrix, hMatrix * gMatrixTransposed);
            Assert.Equal(halfSizeZeroMatrix, gMatrix * hMatrixTransposed);
        }

        [Theory]
        [MemberData(nameof(OrthogonalIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateOrthogonalIterationFiltersVectors(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
        {
            // When
            var iterationFilterH = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterH);
            var iterationFilterG = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterG);

            // Then
            CheckIterationFiltersVectors(iterationFilterH, iterationFilterG, testCase.Multiplier);
        }

        [Theory]
        [MemberData(nameof(OrthogonalIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateOrthogonalIterationFiltersPolynomials(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
        {
            // Given
            var sourceFilterExpectedDegree = testCase.SourceFilterH.Length - 1;
            var hFilterPolynomial = new Polynomial(testCase.SourceFilterH);
            var gFilterPolynomial = new Polynomial(testCase.SourceFilterG);

            // When
            var iterationFilterH = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, hFilterPolynomial, sourceFilterExpectedDegree);
            var iterationFilterG = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, gFilterPolynomial, sourceFilterExpectedDegree);

            // Then
            var iterationFilterExpectedDegree = testCase.SourceFilterH.Length / 2.Pow(testCase.IterationNumber - 1) - 1;
            CheckIterationFiltersVectors(
                iterationFilterH.GetCoefficients(iterationFilterExpectedDegree),
                iterationFilterG.GetCoefficients(iterationFilterExpectedDegree),
                testCase.Multiplier
            );
        }
    }
}