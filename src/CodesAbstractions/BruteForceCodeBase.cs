namespace AppliedAlgebra.CodesAbstractions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Comparers;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public abstract class BruteForceCodeBase : ICode
    {
        private readonly Dictionary<FieldElement[], FieldElement[]> _codeByInformation;

        public GaloisField Field { get; }
        public int CodewordLength { get; }
        public int InformationWordLength { get; }
        public int CodeDistance { get; }

        protected abstract FieldElement[] GenerateCodeword(FieldElement[] informationWord);

        protected void GenerateCodewords(IList<int> informationWord, int currentPosition)
        {
            if (currentPosition == InformationWordLength)
            {
                var preparedInformationWord = informationWord.Select(x => Field.CreateElement(x)).ToArray();
                _codeByInformation[preparedInformationWord] = GenerateCodeword(preparedInformationWord);
            }
            else
                for (var i = 0; i < Field.Order; i++)
                {
                    informationWord[currentPosition] = i;
                    GenerateCodewords(informationWord, currentPosition + 1);
                }
        }

        protected BruteForceCodeBase(GaloisField field, int codewordLength, int informationWordLength, int codeDistance)
        {
            Field = field;
            CodewordLength = codewordLength;
            InformationWordLength = informationWordLength;
            CodeDistance = codeDistance;

            _codeByInformation = new Dictionary<FieldElement[], FieldElement[]>(new FieldElementsArraysComparer());
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

            return _codeByInformation[informationWord];
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
                .Select(x => x.Key)
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
    }
}