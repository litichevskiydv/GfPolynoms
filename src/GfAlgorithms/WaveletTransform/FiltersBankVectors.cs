namespace AppliedAlgebra.GfAlgorithms.WaveletTransform
{
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Filters bank definition in the vector style
    /// </summary>
    public class FiltersBankVectors
    {
        /// <summary>
        /// Analysis filters pair
        /// </summary>
        public (FieldElement[] hWithTilde, FieldElement[] gWithTilde) AnalysisPair { get; }

        /// <summary>
        /// Synthesis filters pair
        /// </summary>
        public (FieldElement[] h, FieldElement[] g) SynthesisPair { get; }

        public FiltersBankVectors(
            (FieldElement[] hWithTilde, FieldElement[] gWithTilde) analysisPair,
            (FieldElement[] h, FieldElement[] g) synthesisPair
        )
        {
            AnalysisPair = analysisPair;
            SynthesisPair = synthesisPair;
        }

        public void Deconstruct(
            out (FieldElement[] hWithTilde, FieldElement[] gWithTilde) analysisPair,
            out (FieldElement[] h, FieldElement[] g) synthesisPair
        )
        {
            analysisPair = AnalysisPair;
            synthesisPair = SynthesisPair;
        }

        private static string FilterToString(IEnumerable<FieldElement> filter) =>
            string.Join(",", filter);

        public override string ToString()
        {
            return $"AnalysisPair: (hWithTilde: {FilterToString(AnalysisPair.hWithTilde)}; gWithTilde: {FilterToString(AnalysisPair.gWithTilde)}), " +
                   $"SynthesisPair: (h: {FilterToString(SynthesisPair.h)}; g: {FilterToString(SynthesisPair.g)})";
        }
    }
}