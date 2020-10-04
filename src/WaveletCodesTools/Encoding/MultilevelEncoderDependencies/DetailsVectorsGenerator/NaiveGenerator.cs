namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Implementation of a details vectors generator for codes that obtains details vector as a result
    /// of the linear transformation of the approximation vector
    /// </summary>
    public class NaiveGenerator : IDetailsVectorsGenerator
    {
        private readonly IReadOnlyList<FieldElementsMatrix> _levelsTransforms;

        public NaiveGenerator(params FieldElementsMatrix[] levelsTransforms)
        {
            if (levelsTransforms == null)
                throw new ArgumentNullException(nameof(levelsTransforms));

            _levelsTransforms = levelsTransforms;
        }

        public FieldElement[] GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElement[] approximationVector)
        {
            if(levelNumber < 0)
                throw new ArgumentException($"{nameof(levelNumber)} must not be negative");
            if(levelNumber >= _levelsTransforms.Count)
                throw new ArgumentException($"Level {levelNumber} of transformation is not supported");

            return (_levelsTransforms[levelNumber] * FieldElementsMatrix.ColumnVector(approximationVector)).GetColumn(0);
        }
    }
}