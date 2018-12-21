namespace AppliedAlgebra.GolayCodesTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodesAbstractions;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public abstract class GolayCodeBase : ICode
    {
        private readonly Dictionary<Polynomial, FieldElement[]> _codeByInformation;
        private readonly Dictionary<Polynomial, FieldElement[]> _informationWordsByPolynomials;

        public GaloisField Field { get; }
        public int CodewordLength { get; }
        public int InformationWordLength { get; }
        public int CodeDistance { get; }

        private void GenerateCodewords(
            Polynomial generatingPolynomial,
            Polynomial modularPolynomial,
            int[] informationWord,
            int currentPosition)
        {
            if (currentPosition == InformationWordLength)
            {
                var informationPolynomial = new Polynomial(Field, informationWord);
                _informationWordsByPolynomials[informationPolynomial] = informationWord.Select(x => Field.CreateElement(x)).ToArray();

                var codePolynomial = informationPolynomial * generatingPolynomial % modularPolynomial;
                if (CodewordLength % 2 == 0)
                    codePolynomial += new Polynomial(Field, informationPolynomial.Evaluate(1)).RightShift(CodewordLength - 1);

                _codeByInformation[informationPolynomial] = codePolynomial.GetCoefficients(CodewordLength - 1);
                return;
            }

            for (var i = 0; i < Field.Order; i++)
            {
                informationWord[currentPosition] = i;
                GenerateCodewords(generatingPolynomial, modularPolynomial, informationWord, currentPosition + 1);
            }
        }

        protected GolayCodeBase(PrimeOrderField field, int codewordLength, int informationWordLength, int codeDistance)
        {
            Field = field;
            CodewordLength = codewordLength;
            InformationWordLength = informationWordLength;
            CodeDistance = codeDistance;

            _codeByInformation = new Dictionary<Polynomial, FieldElement[]>();
            _informationWordsByPolynomials = new Dictionary<Polynomial, FieldElement[]>();
            var modularPolynomialDegree = CodewordLength - (CodewordLength % 2 == 0 ? 1 : 0);
            GenerateCodewords(
                new Polynomial(Field, 2, 2, 1, 2, 2, 2, 1, 1, 1, 2, 1),
                new Polynomial(Field, 1).RightShift(modularPolynomialDegree) + new Polynomial(Field, Field.InverseForAddition(1)),
                new int[InformationWordLength],
                0
            );
        }

        public FieldElement[] Encode(FieldElement[] informationWord)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (informationWord.Length != InformationWordLength)
                throw new ArgumentNullException($"{nameof(informationWord)} length must be {InformationWordLength}");
            if (informationWord.Any(x => x == null))
                throw new ArgumentException($"{nameof(informationWord)} mustn't contains null elements");
            if (informationWord.Any(x => x.Field.Equals(Field) == false))
                throw new ArgumentException($"All elements of {nameof(informationWord)} must belong to field {Field}");

            return _codeByInformation[new Polynomial(informationWord)];
        }

        private IReadOnlyList<FieldElement[]> PerformListDecoding(int listDecodingRadius, FieldElement[] noisyCodeword)
        {
            if (noisyCodeword == null)
                throw new ArgumentNullException(nameof(noisyCodeword));
            if (noisyCodeword.Length != CodewordLength)
                throw new ArgumentNullException($"{nameof(noisyCodeword)} length must be {CodewordLength}");
            if (noisyCodeword.Any(x => x == null))
                throw new ArgumentException($"{nameof(noisyCodeword)} mustn't contains null elements");
            if (noisyCodeword.Any(x => x.Field.Equals(Field) == false))
                throw new ArgumentException($"All elements of {nameof(noisyCodeword)} must belong to field {Field}");

            return _codeByInformation
                .Where(x => x.Value.ComputeHammingDistance(noisyCodeword) <= listDecodingRadius)
                .Select(x => _informationWordsByPolynomials[x.Key])
                .ToArray();
        }

        public FieldElement[] Decode(FieldElement[] noisyCodeword)
        {
            var listDecodingResult = PerformListDecoding((CodeDistance - 1) / 2, noisyCodeword);
            if (listDecodingResult.Count != 1)
                throw new InvalidOperationException("Decoding can't be performed");

            return listDecodingResult[0];
        }

        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
        {
            return PerformListDecoding(listDecodingRadius ?? (CodeDistance - 1) / 2 + 1, noisyCodeword);
        }

        public override string ToString()
        {
            return $"G{CodewordLength}[{CodewordLength},{InformationWordLength},{CodeDistance}]";
        }
    }
}