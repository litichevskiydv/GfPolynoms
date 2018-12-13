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

    public class G12GolayCode : ICode
    {
        private readonly PrimeOrderField _field;
        private readonly int _listDecodingRadius;
        private readonly Dictionary<Polynomial, FieldElement[]> _codeByInformation;

        public GaloisField Field => _field;

        public int CodewordLength => 12;
        public int InformationWordLength => 6;
        public int CodeDistance => 6;

        private void GenerateCodewords(
            Polynomial generatingPolynomial,
            Polynomial modularPolynomial,
            int[] informationWord,
            int currentPosition)
        {
            if (currentPosition == InformationWordLength)
            {
                var informationPolynomial = new Polynomial(Field, informationWord);

                var codePolynomial = informationPolynomial * generatingPolynomial % modularPolynomial;
                codePolynomial += new Polynomial(Field, informationPolynomial.Evaluate(1)).RightShift(CodewordLength - 1);

                _codeByInformation[informationPolynomial] = codePolynomial.GetCoefficients();
                return;
            }

            for (var i = 0; i < Field.Order; i++)
            {
                informationWord[currentPosition] = i;
                GenerateCodewords(generatingPolynomial, modularPolynomial, informationWord, currentPosition + 1);
            }
        }

        public G12GolayCode(int listDecodingRadius)
        {
            _field = new PrimeOrderField(3);
            _listDecodingRadius = listDecodingRadius;

            _codeByInformation = new Dictionary<Polynomial, FieldElement[]>();
            GenerateCodewords(
                new Polynomial(Field, 2, 2, 1, 2, 2, 2, 1, 1, 1, 2, 1),
                new Polynomial(Field, 1).RightShift(CodewordLength - 2) + new Polynomial(Field, Field.InverseForAddition(1)),
                new int[InformationWordLength],
                0
            );
        }


        public FieldElement[] Encode(FieldElement[] informationWord)
        {
            if(informationWord == null)
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
            if(noisyCodeword == null)
                throw new ArgumentNullException(nameof(noisyCodeword));
            if (noisyCodeword.Length != InformationWordLength)
                throw new ArgumentNullException($"{nameof(noisyCodeword)} length must be {CodewordLength}");
            if (noisyCodeword.Any(x => x == null))
                throw new ArgumentException($"{nameof(noisyCodeword)} mustn't contains null elements");
            if (noisyCodeword.Any(x => x.Field.Equals(Field) == false))
                throw new ArgumentException($"All elements of {nameof(noisyCodeword)} must belong to field {Field}");

            return _codeByInformation.Values
                .Where(x => x.ComputeHammingDistance(noisyCodeword) <= listDecodingRadius)
                .ToArray();
        }

        public FieldElement[] Decode(FieldElement[] noisyCodeword)
        {
            var listDecodingResult = PerformListDecoding((CodeDistance - 1) / 2, noisyCodeword);
            if(listDecodingResult.Count != 1)
                throw new InvalidOperationException("Decoding can't be performed");

            return listDecodingResult[0];
        }

        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword)
        {
            return PerformListDecoding(_listDecodingRadius, noisyCodeword);
        }
    }
}