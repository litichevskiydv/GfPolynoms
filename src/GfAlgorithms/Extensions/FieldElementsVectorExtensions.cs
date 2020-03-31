namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public static class FieldElementsVectorExtensions
    {
        /// <summary>
        /// Computes the Hamming distance between codewords <paramref name="first"/> and <paramref name="second"/>
        /// </summary>
        public static int ComputeHammingDistance(this FieldElement[] first, FieldElement[] second)
        {
            if(first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (first.Length != second.Length)
                throw new InvalidOperationException("Codewords must have the same length");

            var distance = 0;
            for (var i = 0; i < first.Length; i++)
            {
                if(first[i].Field.Equals(second[i].Field) == false)
                    throw new InvalidOperationException("Fields of the codewords must be the same");

                if (first[i].Equals(second[i]) == false)
                    distance++;
            }

            return distance;
        }

        /// <summary>
        /// Adds noise <paramref name="additiveNoise"/> to vector <paramref name="vector"/>
        /// </summary>
        public static FieldElement[] AddNoise(this FieldElement[] vector, FieldElement[] additiveNoise)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));
            if (additiveNoise == null)
                throw new ArgumentNullException(nameof(additiveNoise));
            if (vector.Length != additiveNoise.Length)
                throw new InvalidOperationException("Codewords must have the same length");

            return vector.Select((x, i) => x + additiveNoise[i]).ToArray();
        }

        /// <summary>
        /// Computes direct Fourier transform for the given vector <paramref name="vector"/>
        /// </summary>
        public static FieldElement[] GetSpectrum(this FieldElement[] vector)
        {
            if(vector == null)
                throw new ArgumentNullException(nameof(vector));
            if(vector.Length == 0)
                throw new ArgumentNullException($"{nameof(vector)} must not be empty");
            if (vector.Any(x => x == null))
                throw new ArgumentNullException($"{nameof(vector)} components must not be null");

            var field = vector[0].Field;
            if (vector.Any(x => field.Equals(x.Field) == false))
                throw new ArgumentException($"{nameof(vector)} components must belong to the one field");

            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(vector.Length);
            var primitiveRoot = fieldExtension.GetPrimitiveRoot(vector.Length);
            var preparedVector = vector.Select(x => x.TransferToSubfield(fieldExtension)).Reverse().ToArray();

            return Enumerable.Range(0, vector.Length)
                .Select(x =>
                        {
                            var argument = FieldElement.Pow(primitiveRoot, x);
                            return preparedVector.Aggregate(
                                fieldExtension.Zero(),
                                (spectrumComponent, component) => component + spectrumComponent * argument
                            );
                        })
                .ToArray();
        }
    }
}