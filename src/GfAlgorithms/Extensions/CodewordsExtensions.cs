namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;
    using GfPolynoms;

    public static class CodewordsExtensions
    {
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
    }
}