namespace AppliedAlgebra.CodesAbstractions
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;


    public class BasicCodewordMutator : ICodewordMutator
    {
        public FieldElement[] Mutate(FieldElement[] originalCodeword, int requiredLength)
        {
            if (originalCodeword == null)
                throw new ArgumentNullException(nameof(originalCodeword));
            if (originalCodeword.Length == 0)
                throw new ArgumentException($"{nameof(originalCodeword)} must not be empty");
            if (requiredLength <= 0)
                throw new ArgumentException($"{requiredLength} must be positive");

            var field = originalCodeword.GetField();
            if (originalCodeword.Length + 1 == requiredLength)
                return originalCodeword.Concat(new[] {originalCodeword.Aggregate(field.Zero(), (checkSum, x) => checkSum + x)}).ToArray();
            if (originalCodeword.Length == requiredLength)
                return originalCodeword;
            if (originalCodeword.Length > requiredLength)
                return originalCodeword.Take(requiredLength).ToArray();

            throw new InvalidOperationException("Codeword length can't be adjusted to requirements");
        }
    }
}