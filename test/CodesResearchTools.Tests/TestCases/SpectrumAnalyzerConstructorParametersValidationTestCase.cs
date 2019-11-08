namespace AppliedAlgebra.CodesResearchTools.Tests.TestCases
{
    using Analyzers.Spectrum;
    using GfAlgorithms.VariantsIterator;
    using Microsoft.Extensions.Logging;

    public class SpectrumAnalyzerConstructorParametersValidationTestCase
    {
        public IVariantsIterator VariantsIterator { get; set; }

        public ILogger<CommonSpectrumAnalyzer> Logger { get; set; }
    }
}