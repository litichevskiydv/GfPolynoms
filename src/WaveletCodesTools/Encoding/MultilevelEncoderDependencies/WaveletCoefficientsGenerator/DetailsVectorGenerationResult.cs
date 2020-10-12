namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using GfAlgorithms.Matrices;

    public class DetailsVectorGenerationResult
    {
        /// <summary>
        /// Details vector defined as a column vector
        /// </summary>
        public FieldElementsMatrix DetailsVector { get; }

        /// <summary>
        /// Number of the correctable components in the end of the details vector
        /// </summary>
        public int CorrectableComponentsCount { get; }

        /// <summary>
        /// Details vector generation result
        /// </summary>
        /// <param name="detailsVector">Details vector defined as a column vector</param>
        /// <param name="correctableComponentsCount">Number of the correctable components in the end of the details vector</param>
        public DetailsVectorGenerationResult(FieldElementsMatrix detailsVector, int correctableComponentsCount)
        {
            if(detailsVector == null)
                throw new ArgumentNullException(nameof(detailsVector));
            if(detailsVector.ColumnsCount != 1)
                throw new ArgumentException($"{nameof(detailsVector)} must be a column vector");
            if(correctableComponentsCount < 0)
                throw new ArgumentException($"{nameof(correctableComponentsCount)} must not be negative");
            if(correctableComponentsCount > detailsVector.RowsCount)
                throw new ArgumentException($"{nameof(correctableComponentsCount)} must not be greater than {nameof(detailsVector)} length");

            DetailsVector = detailsVector;
            CorrectableComponentsCount = correctableComponentsCount;
        }

        public void Deconstruct(out FieldElementsMatrix detailsVector, out int correctableComponentsCount)
        {
            detailsVector = DetailsVector;
            correctableComponentsCount = CorrectableComponentsCount;
        }
    }
}