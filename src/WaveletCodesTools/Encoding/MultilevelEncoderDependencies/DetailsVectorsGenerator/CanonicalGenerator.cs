namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// An generator of an approximation vector for codes that sequentially transforms parts of an information word
    /// </summary>
    public class CanonicalGenerator : IDetailsVectorsGenerator
    {
        public FieldElement[] GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElement[] approximationVector)
        {
            if(informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var field = approximationVector.GetField();
            var informationSymbols = informationWord.Skip(approximationVector.Length).Take(approximationVector.Length).ToArray();
            return informationSymbols.Concat(Enumerable.Repeat(field.Zero(), approximationVector.Length - informationSymbols.Length))
                .ToArray();
        }
    }
}