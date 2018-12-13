namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;
    using System.Linq;
    using GfPolynoms;

    public static class CodewordsExtensions
    {
        /// <summary>
        /// Method for computing the Hamming distance between codewords <paramref name="first"/> and <paramref name="second"/>
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
        /// Method for adding noise <paramref name="additiveNoise"/> to codeword <paramref name="codeword"/>
        /// </summary>
        public static FieldElement[] AddNoise(this FieldElement[] codeword, FieldElement[] additiveNoise)
        {
            if (codeword == null)
                throw new ArgumentNullException(nameof(codeword));
            if (additiveNoise == null)
                throw new ArgumentNullException(nameof(additiveNoise));
            if (codeword.Length != additiveNoise.Length)
                throw new InvalidOperationException("Codewords must have the same length");

            return codeword.Select((x, i) => x + additiveNoise[i]).ToArray();
        }
    }
}