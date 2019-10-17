namespace AppliedAlgebra.GfAlgorithms.VariantsIterator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Objects iterator recursive implementation
    /// </summary>
    public class RecursiveIterator : IVariantsIterator
    {
        private static IEnumerable<TResult> Iterate<TResult>(GaloisField field, int currentPosition, int[] values, Func<int[], TResult> resultProducer)
        {
            if (currentPosition < 0)
            {
                yield return resultProducer(values);
                yield break;
            }

            for (var i = 0; i < field.Order; i++)
            {
                values[currentPosition] = i;
                foreach (var computedValues in Iterate(field, currentPosition - 1, values, resultProducer))
                    yield return computedValues;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FieldElement[]> IterateVectors(GaloisField field, int length)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (length <= 0)
                throw new ArgumentException($"{nameof(length)} must be positive");

            return Iterate(field, length - 1, new int[length], values => values.Select(field.CreateElement).ToArray());
        }

        /// <inheritdoc />
        public IEnumerable<Polynomial> IteratePolynomials(GaloisField field, int maxDegree)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (maxDegree < 0)
                throw new ArgumentException($"{nameof(maxDegree)} must not be negative");

            return Iterate(field, maxDegree, new int[maxDegree + 1], values => new Polynomial(field, values));
        }
    }
}