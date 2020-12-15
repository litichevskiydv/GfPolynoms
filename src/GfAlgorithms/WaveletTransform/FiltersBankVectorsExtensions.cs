namespace AppliedAlgebra.GfAlgorithms.WaveletTransform
{
    using System;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using Matrices;

    public static class FiltersBankVectorsExtensions
    {
        private static bool IsSatisfy(
            this FiltersBankVectors filtersBank,
            FieldElement multiplier,
            Func<FieldElement, FieldElementsMatrix, FieldElementsMatrix, FieldElementsMatrix, FieldElementsMatrix, bool> condition
        )
        {
            if (filtersBank == null)
                throw new ArgumentNullException(nameof(filtersBank));

            var field = filtersBank.SynthesisPair.h.GetField();
            if (multiplier != null && multiplier.Field.Equals(field) == false)
                throw new ArgumentException($"{multiplier} belongs to the wrong field");

            var checkedMultiplier = multiplier != null ? FieldElement.InverseForMultiplication(multiplier) : field.One();
            var hMatrixTransposed = FieldElementsMatrix.DoubleCirculantMatrix(filtersBank.SynthesisPair.h).Transpose();
            var gMatrixTransposed = FieldElementsMatrix.DoubleCirculantMatrix(filtersBank.SynthesisPair.g).Transpose();
            var hMatrixWithTilde = FieldElementsMatrix.DoubleCirculantMatrix(filtersBank.AnalysisPair.hWithTilde);
            var gMatrixWithTilde = FieldElementsMatrix.DoubleCirculantMatrix(filtersBank.AnalysisPair.gWithTilde);

            return condition(checkedMultiplier, hMatrixTransposed, gMatrixTransposed, hMatrixWithTilde, gMatrixWithTilde);
        }

        /// <summary>
        /// checks if the filter bank can perform perfect reconstruction
        /// </summary>
        /// <param name="filtersBank">Verifiable filter bank</param>
        /// <param name="multiplier">Condition multiplier</param>
        public static bool CanPerformPerfectReconstruction(this FiltersBankVectors filtersBank, FieldElement multiplier = null) =>
            IsSatisfy(
                filtersBank,
                multiplier,
                (checkedMultiplier, hMatrixTransposed, gMatrixTransposed, hMatrixWithTilde, gMatrixWithTilde) =>
                    (checkedMultiplier * (hMatrixTransposed * hMatrixWithTilde + gMatrixTransposed * gMatrixWithTilde)).IsIdentity()
            );

        /// <summary>
        /// checks if the filter bank satisfy biorthogonal condition
        /// </summary>
        /// <param name="filtersBank">Verifiable filter bank</param>
        /// <param name="multiplier">Condition multiplier</param>
        public static bool IsSatisfyBiorthogonalCondition(this FiltersBankVectors filtersBank, FieldElement multiplier = null) =>
            IsSatisfy(
                filtersBank,
                multiplier,
                (checkedMultiplier, hMatrixTransposed, gMatrixTransposed, hMatrixWithTilde, gMatrixWithTilde) =>
                    (checkedMultiplier * hMatrixWithTilde * hMatrixTransposed).IsIdentity() &&
                    (checkedMultiplier * gMatrixWithTilde * gMatrixTransposed).IsIdentity() &&
                    (hMatrixWithTilde * gMatrixTransposed).IsZero() &&
                    (gMatrixWithTilde * hMatrixTransposed).IsZero()
            );
    }
}