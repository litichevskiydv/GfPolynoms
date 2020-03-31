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

        /// <summary>
        /// Computes direct Fourier transform for the given signal <paramref name="signal"/>
        /// </summary>
        public static FieldElement[] GetSpectrum(this FieldElement[] signal)
        {
            if(signal == null)
                throw new ArgumentNullException(nameof(signal));
            if(signal.Length == 0)
                throw new ArgumentNullException($"{nameof(signal)} must not be empty");
            if (signal.Any(x => x == null))
                throw new ArgumentNullException($"{nameof(signal)} components must not be null");

            var field = signal[0].Field;
            if (signal.Any(x => field.Equals(x.Field) == false))
                throw new ArgumentException($"{nameof(signal)} components must belong to the one field");

            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(signal.Length);
            var primitiveRoot = fieldExtension.GetPrimitiveRoot(signal.Length);
            var preparedSignal = signal.Select(x => x.TransferToSubfield(fieldExtension)).Reverse().ToArray();

            return Enumerable.Range(0, signal.Length)
                .Select(x =>
                        {
                            var argument = FieldElement.Pow(primitiveRoot, x);
                            return preparedSignal.Aggregate(
                                fieldExtension.Zero(),
                                (spectrumComponent, component) => component + spectrumComponent * argument
                            );
                        })
                .ToArray();
        }

        /// <summary>
        /// Computes reverse Fourier transform for the given spectrum <paramref name="spectrum"/>
        /// </summary>
        public static FieldElement[] GetSignal(this FieldElement[] spectrum)
        {
            if (spectrum == null)
                throw new ArgumentNullException(nameof(spectrum));
            if (spectrum.Length == 0)
                throw new ArgumentNullException($"{nameof(spectrum)} must not be empty");
            if (spectrum.Any(x => x == null))
                throw new ArgumentNullException($"{nameof(spectrum)} components must not be null");

            var field = spectrum[0].Field;
            if (spectrum.Any(x => field.Equals(x.Field) == false))
                throw new ArgumentException($"{nameof(spectrum)} components must belong to the one field");

            var primitiveRoot = field.GetPrimitiveRoot(spectrum.Length);
            var multiplier = field.CreateElement(spectrum.Length % field.Characteristic).InverseForMultiplication();
            var preparedSpectrum = spectrum.Reverse().ToArray();

            return Enumerable.Range(0, spectrum.Length)
                .Select(x =>
                        {
                            var argument = FieldElement.Pow(primitiveRoot, -x);
                            return multiplier * preparedSpectrum.Aggregate(
                                field.Zero(),
                                (signalComponent, component) => component + signalComponent * argument
                            );
                        })
                .ToArray();
        }
    }
}