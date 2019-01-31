namespace AppliedAlgebra.WaveletCodesTools.FixedDistanceCodesFactory
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

        internal FixedDistanceWaveletCode(
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

        /// <inheritdoc />
        public FieldElement[] Encode(FieldElement[] informationWord) =>
            (new Polynomial(informationWord).RaiseVariableDegree(2) * _generatingPolynomial % _modularPolynomial)
            .GetCoefficients(CodewordLength - 1);

        private Tuple<FieldElement, FieldElement>[] TransformNoisyCodeword(FieldElement[] noisyCodeword) =>
            noisyCodeword.Select((x, i) => Tuple.Create(_preparedPoints[i], x)).ToArray();

        /// <inheritdoc />
        public FieldElement[] Decode(FieldElement[] noisyCodeword) =>
            _decoder.Decode(
                    CodewordLength,
                    InformationWordLength,
                    CodeDistance,
                    _generatingPolynomial,
                    TransformNoisyCodeword(noisyCodeword)
                )
                .GetCoefficients(InformationWordLength - 1);

        /// <inheritdoc />
        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null) =>
            _listDecoder.Decode(CodewordLength,
                    InformationWordLength,
                    CodeDistance,
                    _generatingPolynomial,
                    TransformNoisyCodeword(noisyCodeword),
                    CodewordLength - Math.Min(_maxListDecodingRadius, listDecodingRadius ?? _maxListDecodingRadius)
                )
                .Select(x => x.GetCoefficients(InformationWordLength - 1))
                .ToArray();
    }
}