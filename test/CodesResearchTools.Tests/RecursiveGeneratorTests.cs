namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System.Linq;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using NoiseGenerator;
    using TestCases;
    using Xunit;

    public class RecursiveGeneratorTests
    {
        [UsedImplicitly]
        public static readonly TheoryData<NoisePositionsAndValuesVariationTestCase> VariatePositionsAndValuesTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<NoiseValuesVariationTestCase> VariateValuesTestsData;

        private readonly RecursiveGenerator _noiseGenerator;

        static RecursiveGeneratorTests()
        {
            var gf3 = GaloisField.Create(3);

            VariatePositionsAndValuesTestsData
                = new TheoryData<NoisePositionsAndValuesVariationTestCase>
                  {
                      new NoisePositionsAndValuesVariationTestCase
                      {
                          Field = gf3,
                          CodewordLength = 3,
                          ErrorsCount = 2,
                          Expected
                              = new[]
                                {
                                    new[] {gf3.CreateElement(1), gf3.CreateElement(1), gf3.Zero()},
                                    new[] {gf3.CreateElement(1), gf3.CreateElement(2), gf3.Zero()},
                                    new[] {gf3.CreateElement(2), gf3.CreateElement(1), gf3.Zero()},
                                    new[] {gf3.CreateElement(2), gf3.CreateElement(2), gf3.Zero()},

                                    new[] {gf3.CreateElement(1), gf3.Zero(), gf3.CreateElement(1)},
                                    new[] {gf3.CreateElement(1), gf3.Zero(), gf3.CreateElement(2)},
                                    new[] {gf3.CreateElement(2), gf3.Zero(), gf3.CreateElement(1)},
                                    new[] {gf3.CreateElement(2), gf3.Zero(), gf3.CreateElement(2)},

                                    new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(1)},
                                    new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(2)},
                                    new[] {gf3.Zero(), gf3.CreateElement(2), gf3.CreateElement(1)},
                                    new[] {gf3.Zero(), gf3.CreateElement(2), gf3.CreateElement(2)}
                                }
                      },
                      new NoisePositionsAndValuesVariationTestCase
                      {
                          Field = gf3,
                          CodewordLength = 3,
                          ErrorsCount = 2,
                          InitialNoiseValue = new[] {gf3.CreateElement(1), gf3.Zero(), gf3.CreateElement(2)},
                          Expected
                              = new[]
                                {
                                    new[] {gf3.CreateElement(1), gf3.Zero(), gf3.CreateElement(2)},
                                    new[] {gf3.CreateElement(2), gf3.Zero(), gf3.CreateElement(1)},
                                    new[] {gf3.CreateElement(2), gf3.Zero(), gf3.CreateElement(2)},

                                    new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(1)},
                                    new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(2)},
                                    new[] {gf3.Zero(), gf3.CreateElement(2), gf3.CreateElement(1)},
                                    new[] {gf3.Zero(), gf3.CreateElement(2), gf3.CreateElement(2)}
                                }
                      }
                  };
            VariateValuesTestsData
                = new TheoryData<NoiseValuesVariationTestCase>
                  {
                      new NoiseValuesVariationTestCase
                      {
                          InitialNoiseValue = new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(1)},
                          Expected
                              = new[]
                                {
                                    new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(1)},
                                    new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(2)},
                                    new[] {gf3.Zero(), gf3.CreateElement(2), gf3.CreateElement(1)},
                                    new[] {gf3.Zero(), gf3.CreateElement(2), gf3.CreateElement(2)}
                                }
                      }
                  };
        }

        public RecursiveGeneratorTests()
        {
            _noiseGenerator = new RecursiveGenerator();
        }

        [Theory]
        [MemberData(nameof(VariatePositionsAndValuesTestsData))]
        public void ShouldTestVariatePositionsAndValues(NoisePositionsAndValuesVariationTestCase testCase)
        {
            // When
            var actual = _noiseGenerator
                .VariatePositionsAndValues(testCase.Field, testCase.CodewordLength, testCase.ErrorsCount, testCase.InitialNoiseValue)
                .ToArray();

            // Then
            Assert.Equal(testCase.Expected.Length, actual.Length);
            Assert.All(actual, x => Assert.Contains(testCase.Expected, y => y.SequenceEqual(x)));
        }

        [Theory]
        [MemberData(nameof(VariateValuesTestsData))]
        public void ShouldTestVariateValues(NoiseValuesVariationTestCase testCase)
        {
            // When
            var actual = _noiseGenerator.VariateValues(testCase.InitialNoiseValue).ToArray();

            // Then
            Assert.Equal(testCase.Expected.Length, actual.Length);
            Assert.All(actual, x => Assert.Contains(testCase.Expected, y => y.SequenceEqual(x)));
        }
    }
}