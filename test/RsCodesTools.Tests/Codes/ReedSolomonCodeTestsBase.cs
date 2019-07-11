namespace AppliedAlgebra.RsCodesTools.Tests.Codes
{
    using System.Collections.Generic;
    using System.Linq;
    using CodesAbstractions;
    using CodesResearchTools.NoiseGenerator;
    using GfPolynoms.Extensions;
    using TestCases;

    public abstract class ReedSolomonCodeTestsBase
    {
        protected static CodewordDecodingTestCase PrepareDecodingTestCase(
            ICode code,
            INoiseGenerator noiseGenerator,
            int? errorsCount = null,
            IEnumerable<int> informationWord = null) =>
            new CodewordDecodingTestCase
            {
                Code = code,
                InformationWord = informationWord?.Select(x => code.Field.CreateElement(x)).ToArray()
                                  ?? Enumerable.Repeat(code.Field.Zero(), code.InformationWordLength).ToArray(),
                AdditiveNoise = noiseGenerator
                    .VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount ?? (code.CodeDistance - 1) / 2)
                    .Skip(30)
                    .First()
            };

        protected static CodewordDecodingTestCase PrepareListDecodingTestCase(
            ICode code,
            INoiseGenerator noiseGenerator,
            IEnumerable<int> informationWord = null) =>
            PrepareDecodingTestCase(code, noiseGenerator, (code.CodeDistance - 1) / 2 + 1, informationWord);
    }
}