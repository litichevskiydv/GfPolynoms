namespace AppliedAlgebra.RsCodesTools.CodesFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodesAbstractions;
    using Decoding.ListDecoder;
    using Decoding.StandartDecoder;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class StandardReedSolomonCode : ICode
    {
        private readonly FieldElement[] _preparedPoints;
        private readonly int _maxListDecodingRadius;

        private readonly IDecoder _decoder;
        private readonly IListDecoder _listDecoder;

        public GaloisField Field { get; }
        public int CodewordLength { get; }
        public int InformationWordLength { get; }
        public int CodeDistance { get; }

        internal StandardReedSolomonCode(
            GaloisField field, 
            int codewordLength, 
            int informationWordLength, 
            IDecoder decoder,
            IListDecoder listDecoder)
        {
            Field = field;
            CodewordLength = codewordLength;
            InformationWordLength = informationWordLength;
            CodeDistance = CodewordLength - InformationWordLength + 1;

            _decoder = decoder;
            _listDecoder = listDecoder;

            _preparedPoints = Enumerable.Range(0, CodewordLength)
                .Select(x => Field.CreateElement(Field.PowGeneratingElement(x)))
                .ToArray();
            _maxListDecodingRadius = (int)Math.Ceiling(CodewordLength - Math.Sqrt(CodewordLength * (CodewordLength - CodeDistance)) - 1);
        }

        public FieldElement[] Encode(FieldElement[] informationWord)
        {
            var informationPolynomial = new Polynomial(informationWord);
            return _preparedPoints.Select(x => Field.CreateElement(informationPolynomial.Evaluate(x.Representation))).ToArray();
        }

        private (FieldElement xValue, FieldElement yValue)[] TransformNoisyCodeword(IEnumerable<FieldElement> noisyCodeword) =>
            noisyCodeword.Select((x, i) => (_preparedPoints[i], x)).ToArray();

        public FieldElement[] Decode(FieldElement[] noisyCodeword) =>
            _decoder.Decode(CodewordLength, InformationWordLength, TransformNoisyCodeword(noisyCodeword))
                .GetCoefficients(InformationWordLength - 1);

        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null) =>
            _listDecoder.Decode(
                    CodewordLength,
                    InformationWordLength,
                    TransformNoisyCodeword(noisyCodeword),
                    CodewordLength - Math.Min(_maxListDecodingRadius, listDecodingRadius ?? _maxListDecodingRadius)
                )
                .Select(x => x.GetCoefficients(InformationWordLength - 1))
                .ToArray();

        public override string ToString() => $"RS[{CodewordLength},{InformationWordLength}]";
    }
}