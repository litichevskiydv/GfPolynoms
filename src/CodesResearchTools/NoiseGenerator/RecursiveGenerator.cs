namespace AppliedAlgebra.CodesResearchTools.NoiseGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class RecursiveGenerator : INoiseGenerator
    {
        private static IEnumerable<FieldElement[]> PlaceErrors(GaloisField field,
            int codewordLength,
            IReadOnlyList<int> errorsPositions,
            IList<int> noiseValue,
            int currentErrorPosition)
        {
            if (currentErrorPosition == errorsPositions.Count)
            {
                var additiveNoise = Enumerable.Repeat(field.Zero(), codewordLength).ToArray();
                for (var i = 0; i < errorsPositions.Count; i++)
                    additiveNoise[errorsPositions[i]] = field.CreateElement(noiseValue[i]);
                yield return additiveNoise;
                yield break;
            }

            for (var i = noiseValue[currentErrorPosition]; i < field.Order; i++)
            {
                noiseValue[currentErrorPosition] = i;

                foreach (var additiveNoise in PlaceErrors(field, codewordLength, errorsPositions, noiseValue, currentErrorPosition + 1))
                    yield return additiveNoise;
            }
            noiseValue[currentErrorPosition] = 1;
        }

        private static IEnumerable<FieldElement[]> Generate(
            GaloisField field, 
            int codewordLength, 
            int[] errorsPositions, 
            IList<int> noiseValue,
            int currentErrorPosition)
        {
            if (currentErrorPosition == errorsPositions.Length)
            {
                foreach (var additiveNoise in PlaceErrors(field, codewordLength, errorsPositions, noiseValue, 0))
                    yield return additiveNoise;
                yield break;
            }

            for (var i = currentErrorPosition == 0
                    ? errorsPositions[currentErrorPosition]
                    : Math.Max(errorsPositions[currentErrorPosition], errorsPositions[currentErrorPosition - 1] + 1);
                i < codewordLength;
                i++)
            {
                errorsPositions[currentErrorPosition] = i;
                foreach (var additiveNoise in Generate(field, codewordLength, errorsPositions, noiseValue, currentErrorPosition + 1))
                    yield return additiveNoise;
            }
            errorsPositions[currentErrorPosition] = 0;
        }

        /// <inheritdoc />
        public IEnumerable<FieldElement[]> VariatePositionsAndValues(GaloisField field, int codewordLength, int errorsCount, FieldElement[] initialNoiseValue = null)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if(codewordLength <= 0)
                throw new ArgumentException($"{nameof(codewordLength)} must be positive");
            if(errorsCount <= 0)
                throw new ArgumentNullException($"{nameof(errorsCount)} must be positive");
            if(errorsCount > codewordLength)
                throw new ArgumentNullException($"{nameof(errorsCount)} must not be greater than codeword length");
            if (initialNoiseValue != null)
            {
                if(initialNoiseValue.Length != codewordLength)
                    throw new ArgumentException($"Length of {nameof(initialNoiseValue)} must be equal to codeword length");
                if(initialNoiseValue.Count(x => x.Representation != 0) != errorsCount)
                    throw new ArgumentException($"{nameof(initialNoiseValue)} must contain {errorsCount} non-zero elements");
            }

            var errorsPositions = new int[errorsCount];
            var noiseValue = Enumerable.Repeat(1, errorsCount).ToArray();
            if(initialNoiseValue != null)
                for(int i = 0, j = 0; i < codewordLength && j < errorsCount; i++)
                    if (initialNoiseValue[i].Representation != 0)
                    {
                        errorsPositions[j] = i;
                        noiseValue[j] = initialNoiseValue[i].Representation;
                        j++;
                    }

            return Generate(field, codewordLength, errorsPositions, noiseValue, 0);
        }

        /// <inheritdoc />
        public IEnumerable<FieldElement[]> VariateValues(FieldElement[] initialNoiseValue)
        {
            if (initialNoiseValue == null || initialNoiseValue.Length == 0)
                throw new ArgumentNullException(nameof(initialNoiseValue));
            if (initialNoiseValue.All(x => x.Representation == 0))
                throw new ArgumentException($"{nameof(initialNoiseValue)} should contain non-zero elements");

            var errorsPositions = new List<int>();
            var noiseValue = new List<int>();
            for (var i = 0; i < initialNoiseValue.Length; i++)
            {
                if (initialNoiseValue[i].Representation == 0)
                    continue;

                errorsPositions.Add(i);
                noiseValue.Add(initialNoiseValue[i].Representation);
            }

            return PlaceErrors(initialNoiseValue[0].Field, initialNoiseValue.Length, errorsPositions, noiseValue, 0);
        }
    }
}