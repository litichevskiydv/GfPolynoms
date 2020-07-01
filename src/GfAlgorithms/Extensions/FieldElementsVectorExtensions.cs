namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public static class FieldElementsVectorExtensions
    {
        /// <summary>
        /// Creates deep copy of the source vector <paramref name="source"/>
        /// </summary>
        public static FieldElement[] CloneVector(this FieldElement[] source)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(component => new FieldElement(component)).ToArray();
        }

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

        /// <summary>
        /// Determines field of the vector <paramref name="vector"/>
        /// </summary>
        /// <param name="vector">Field elements vector</param>
        public static GaloisField GetField(this FieldElement[] vector)
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

            return field;
        }

        private static FieldElement[] ComputeValues(FieldElement primitiveRoot, IReadOnlyList<FieldElement> vector)
        {
            var field = primitiveRoot.Field;
            var componentsCount = vector.Count;

            var divisorUpperBound = Math.Sqrt(componentsCount) + 1;
            for (var divisor = 2; divisor < divisorUpperBound; divisor++)
            {
                if (componentsCount % divisor != 0) continue;

                var quotient = componentsCount / divisor;
                var primitiveRootPower = FieldElement.Pow(primitiveRoot, divisor);
                var vectorParts = new FieldElement[divisor][].MakeRectangle(quotient);
                var computedValues = new FieldElement[divisor][];
                for (var j = 0; j < divisor; j++)
                {
                    for (var k = 0; k < quotient; k++)
                        vectorParts[j][k] = vector[k * divisor + j];
                    computedValues[j] = ComputeValues(primitiveRootPower, vectorParts[j]);
                }

                var reversedRange = Enumerable.Range(0, divisor).Reverse().ToArray();
                return Enumerable.Range(0, componentsCount)
                    .Select(i =>
                            {
                                var argument = FieldElement.Pow(primitiveRoot, i);
                                return reversedRange
                                    .Aggregate(
                                        field.Zero(),
                                        (spectrumComponent, j) => computedValues[j][i % quotient] + spectrumComponent * argument
                                    );
                            }
                    )
                    .ToArray();
            }

            var reversedVector = vector.Reverse().ToArray();
            return Enumerable.Range(0, componentsCount)
                .Select(x =>
                        {
                            var argument = FieldElement.Pow(primitiveRoot, x);
                            return reversedVector.Aggregate(
                                field.Zero(),
                                (spectrumComponent, component) => component + spectrumComponent * argument
                            );
                        })
                .ToArray();
        }

        /// <summary>
        /// Computes direct Fourier transform for the given signal <paramref name="signal"/>
        /// </summary>
        public static FieldElement[] GetSpectrum(this FieldElement[] signal)
        {
            var field = signal.GetField();
            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(signal.Length);

            var primitiveRoot = fieldExtension.GetPrimitiveRoot(signal.Length);
            var preparedSignal = signal.Select(x => x.TransferToSubfield(fieldExtension)).ToArray();
            return ComputeValues(primitiveRoot, preparedSignal);
        }

        /// <summary>
        /// Computes reverse Fourier transform for the given spectrum <paramref name="spectrum"/>
        /// </summary>
        public static FieldElement[] GetSignal(this FieldElement[] spectrum)
        {
            var field = spectrum.GetField();

            var primitiveRoot = field.GetPrimitiveRoot(spectrum.Length).InverseForMultiplication();
            var multiplier = field.CreateElement(spectrum.Length % field.Characteristic).InverseForMultiplication();
            return ComputeValues(primitiveRoot, spectrum)
                .Select(value => multiplier * value)
                .ToArray();
        }
    }
}