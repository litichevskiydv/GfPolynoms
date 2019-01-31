namespace AppliedAlgebra.WaveletCodesTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodesAbstractions;
    using Decoding.ListDecoderForFixedDistanceCodes;
    using Decoding.StandartDecoderForFixedDistanceCodes;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class FixedDistanceWaveletCode : ICode
    {
        private readonly Polynomial _generatingPolynomial;
        private readonly Polynomial _modularPolynomial;
        private readonly FieldElement[] _preparedPoints;
        private readonly int _maxListDecodingRadius;

        private readonly IDecoder _decoder;
        private readonly IListDecoder _listDecoder;

        public GaloisField Field { get; }
        public int CodewordLength { get; }
        public int InformationWordLength { get; }
        public int CodeDistance { get; }

        public FixedDistanceWaveletCode(
            int codewordLength, 
            int informationWordLength, 
            int codeDistance, 
            Polynomial generatingPolynomial,
            IDecoder decoder,
            IListDecoder listDecoder)
        {
            Field = generatingPolynomial.Field;
            CodewordLength = codewordLength;
            InformationWordLength = informationWordLength;
            CodeDistance = codeDistance;

            _generatingPolynomial = generatingPolynomial;
            _decoder = decoder;
            _listDecoder = listDecoder;

            _modularPolynomial = new Polynomial(Field, 1).RightShift(CodewordLength) + new Polynomial(Field, Field.InverseForAddition(1));
            _preparedPoints = Enumerable.Range(0, CodeDistance)
                .Select(x => Field.CreateElement(Field.GetGeneratingElementPower(x)))
                .ToArray();
            _maxListDecodingRadius = (int) Math.Ceiling(CodewordLength - Math.Sqrt(CodewordLength * (CodewordLength - CodeDistance)) - 1);
        }

        public FieldElement[] Encode(FieldElement[] informationWord)
        {
            var informationPolynomial = new Polynomial(informationWord);
            var codePolynomial = informationPolynomial.RaiseVariableDegree(2) * _generatingPolynomial % _modularPolynomial;
            return codePolynomial.GetCoefficients(CodewordLength - 1);
        }

        private Tuple<FieldElement, FieldElement>[] TransformNoisyCodeword(FieldElement[] noisyCodeword) =>
            noisyCodeword.Select((x, i) => Tuple.Create(_preparedPoints[i], x)).ToArray();

        public FieldElement[] Decode(FieldElement[] noisyCodeword) =>
            _decoder.Decode(
                    CodewordLength,
                    InformationWordLength,
                    CodeDistance,
                    _generatingPolynomial,
                    TransformNoisyCodeword(noisyCodeword)
                )
                .GetCoefficients(InformationWordLength - 1);

        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
        {
            if(listDecodingRadius > _maxListDecodingRadius)
                throw new ArgumentException($"List decoding with radius greater than {_maxListDecodingRadius} is not supported");

            return _listDecoder.Decode(CodewordLength,
                    InformationWordLength,
                    CodeDistance,
                    _generatingPolynomial,
                    TransformNoisyCodeword(noisyCodeword),
                    CodewordLength - (listDecodingRadius ?? _maxListDecodingRadius)
                )
                .Select(x => x.GetCoefficients(InformationWordLength - 1))
                .ToArray();
        }
    }
}