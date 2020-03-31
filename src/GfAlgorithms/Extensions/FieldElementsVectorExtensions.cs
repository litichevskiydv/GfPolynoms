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
        /// Adds noise <paramref name="additiveNoise"/> to signal <paramref name="vector"/>
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

        private static FieldElement[] PerformFourierTransform(this FieldElement[] vector, bool isDirect)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));
            if (vector.Length == 0)
                throw new ArgumentNullException($"{nameof(vector)} must not be empty");
            if (vector.Any(x => x == null))
                throw new ArgumentNullException($"{nameof(vector)} components must not be null");

            var field = vector[0].Field;
            if (vector.Any(x => field.Equals(x.Field) == false))
                throw new ArgumentException($"{nameof(vector)} components must belong to the one field");

            FieldElement multiplier;
            FieldElement primitiveRoot;
            FieldElement[] preparedVector;

            if (isDirect)
            {
                var fieldExtension = field.FindExtensionContainingPrimitiveRoot(vector.Length);

                multiplier = fieldExtension.One();
                primitiveRoot = fieldExtension.GetPrimitiveRoot(vector.Length);
                preparedVector = vector.Select(x => x.TransferToSubfield(fieldExtension)).Reverse().ToArray();
            }
            else
            {
                multiplier = field.CreateElement(vector.Length % field.Characteristic).InverseForMultiplication();
                primitiveRoot = field.GetPrimitiveRoot(vector.Length);
                preparedVector = vector.Reverse().ToArray();
            }

            return Enumerable.Range(0, vector.Length)
                .Select(x =>
                        {
                            var argument = FieldElement.Pow(primitiveRoot, isDirect ? x : -x);
                            return multiplier * preparedVector.Aggregate(
                                primitiveRoot.Field.Zero(),
                                (spectrumComponent, component) => component + spectrumComponent * argument
                            );
                        })
                .ToArray();
        }

        /// <summary>
        /// Computes direct Fourier transform for the given signal <paramref name="signal"/>
        /// </summary>
        public static FieldElement[] GetSpectrum(this FieldElement[] signal) =>
            signal.PerformFourierTransform(true);

        /// <summary>
        /// Computes reverse Fourier transform for the given spectrum <paramref name="spectrum"/>
        /// </summary>
        public static FieldElement[] GetSignal(this FieldElement[] spectrum) =>
            spectrum.PerformFourierTransform(false);
    }
}