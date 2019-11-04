﻿namespace AppliedAlgebra.CodesResearchTools.Tests.CodeDistance
{
    using System;
    using System.Linq;
    using Analyzers.CodeDistance;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Moq;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using TestCases;
    using WaveletCodesTools;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.FixedDistanceCodesFactory;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using Xunit;

    public class LinearCodeDistanceAnalyzerTests
    {
        [UsedImplicitly] public static TheoryData<CodeDistanceAnalyzerTestCase> CodeDistanceAnalysisTestCases;

        static LinearCodeDistanceAnalyzerTests()
        {
            var gaussSolver = new GaussSolver();
            var codesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator()),
                        gaussSolver
                    )
                );
            var wN7K3D4 = codesFactory.CreateN7K3D4();
            var wN8K4D4First = new WaveletCode(8, 4, 4, new Polynomial(new PrimePowerOrderField(9), 2, 0, 1, 2, 1, 1));
            var wN8K4D4Second = new WaveletCode(8, 4, 4, new Polynomial(new PrimePowerOrderField(9), 2, 2, 1, 2, 0, 1));

            CodeDistanceAnalysisTestCases
                = new TheoryData<CodeDistanceAnalyzerTestCase>
                  {
                      new CodeDistanceAnalyzerTestCase
                      {
                          Field = wN7K3D4.Field,
                          InformationWordLength = wN7K3D4.InformationWordLength,
                          EncodingProcedure = x => wN7K3D4.Encode(x.Select(value => wN7K3D4.Field.CreateElement(value)).ToArray()),
                          Expected = wN7K3D4.CodeDistance
                      },
                      new CodeDistanceAnalyzerTestCase
                      {
                          Field = wN8K4D4First.Field,
                          InformationWordLength = wN8K4D4First.InformationWordLength,
                          EncodingProcedure = x => wN8K4D4First.Encode(x.Select(value => wN8K4D4First.Field.CreateElement(value)).ToArray()),
                          Expected = wN8K4D4First.CodeDistance
                      },
                      new CodeDistanceAnalyzerTestCase
                      {
                          Field = wN8K4D4Second.Field,
                          InformationWordLength = wN8K4D4Second.InformationWordLength,
                          EncodingProcedure = x => wN8K4D4Second.Encode(x.Select(value => wN8K4D4Second.Field.CreateElement(value)).ToArray()),
                          Expected = wN8K4D4Second.CodeDistance
                      }
                  };
        }

        private readonly Mock<ILogger<LinearCodeDistanceAnalyzer>> _mockLogger;
        private readonly LinearCodeDistanceAnalyzer _analyzer;

        public LinearCodeDistanceAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<LinearCodeDistanceAnalyzer>>();
            _analyzer = new LinearCodeDistanceAnalyzer(_mockLogger.Object);
        }

        [Theory]
        [MemberData(nameof(CodeDistanceAnalysisTestCases))]
        public void ShouldAnalyzeCodeDistance(CodeDistanceAnalyzerTestCase testCase)
        {
            Assert.Equal(
                testCase.Expected,
                _analyzer.Analyze(
                    testCase.Field, testCase.InformationWordLength, testCase.EncodingProcedure,
                    new CodeDistanceAnalyzerOptions {LoggingResolution = 100}
                )
            );

            Assert.All(_mockLogger.Invocations, x => Assert.Equal(LogLevel.Information, x.Arguments[0]));
            Assert.True(_mockLogger.Invocations.Count >= 1);
        }
    }
}