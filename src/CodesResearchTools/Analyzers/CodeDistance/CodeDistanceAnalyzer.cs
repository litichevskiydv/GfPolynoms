namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using CodesAbstractions;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class CodeDistanceAnalyzer
    {
        private static IEnumerable<(int[] informationWord, FieldElement[] codeword)> GenerateMappings(ICode code, int[] informationWord, int currentPosition)
        {
            if (currentPosition == code.InformationWordLength)
            {
                yield return (informationWord, code.Encode(informationWord.Select(x => code.Field.CreateElement(x)).ToArray()));
                yield break;
            }

            for (var i = informationWord[currentPosition]; i < code.Field.Order; i++)
            {
                informationWord[currentPosition] = i;

                foreach (var mapping in GenerateMappings(code, informationWord, currentPosition + 1))
                    yield return mapping;

            }
            informationWord[currentPosition] = 0;
        }

        public int Analyze(ICode code)
        {
            if(code == null)
                throw new ArgumentNullException(nameof(code));

            var codeDistance = code.CodewordLength;
            foreach (var firstMapping in GenerateMappings(code, new int[code.InformationWordLength], 0))
            foreach (var secondMapping in GenerateMappings(code, firstMapping.informationWord.ToArray(), 0).Skip(1))
                codeDistance = Math.Min(codeDistance, firstMapping.codeword.ComputeHammingDistance(secondMapping.codeword));

            return codeDistance;
        }
    }
}