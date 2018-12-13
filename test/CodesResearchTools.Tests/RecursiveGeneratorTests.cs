﻿namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using NoiseGenerator;
    using Xunit;

    public class RecursiveGeneratorTests
    {
        [UsedImplicitly] public static readonly IEnumerable<object[]> VariatePositionsAndValuesTestsData;
        [UsedImplicitly] public static readonly IEnumerable<object[]> VariateValuesTestsData;

        private readonly RecursiveGenerator _noiseGenerator;

        static RecursiveGeneratorTests()
        {
            var gf3 = new PrimeOrderField(3);

            VariatePositionsAndValuesTestsData
                = new[]
                  {
                      new object[]
                      {
                          gf3, 3, 2, null,
                          new[]
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
                      new object[]
                      {
                          gf3, 3, 2, new[] {gf3.CreateElement(1), gf3.Zero(), gf3.CreateElement(2)},
                          new[]
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
                = new[]
                  {
                      new object[]
                      {
                          new[] {gf3.Zero(), gf3.CreateElement(1), gf3.CreateElement(1)},
                          new[]
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
        public void ShouldTestVariatePositionsAndValues(
            GaloisField field,
            int codewordLength,
            int errorsCount,
            FieldElement[] initialNoiseValue,
            FieldElement[][] expected)
        {
            // When
            var actual = _noiseGenerator.VariatePositionsAndValues(field, codewordLength, errorsCount, initialNoiseValue).ToArray();

            // Then
            Assert.Equal(expected.Length, actual.Length);
            Assert.All(actual, x => Assert.Contains(expected, y => y.SequenceEqual(x)));
        }

        [Theory]
        [MemberData(nameof(VariateValuesTestsData))]
        public void ShouldTestVariateValues(
            FieldElement[] initialNoiseValue,
            FieldElement[][] expected)
        {
            // When
            var actual = _noiseGenerator.VariateValues(initialNoiseValue).ToArray();

            // Then
            Assert.Equal(expected.Length, actual.Length);
            Assert.All(actual, x => Assert.Contains(expected, y => y.SequenceEqual(x)));
        }
    }
}