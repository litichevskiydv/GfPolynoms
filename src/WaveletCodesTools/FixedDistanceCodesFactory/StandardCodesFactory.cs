namespace AppliedAlgebra.WaveletCodesTools.FixedDistanceCodesFactory
{
    using System;
    using CodesAbstractions;
    using Decoding.ListDecoderForFixedDistanceCodes;
    using Decoding.StandartDecoderForFixedDistanceCodes;
    using GeneratingPolynomialsBuilder;
    using GfPolynoms;

    public class StandardCodesFactory : IFixedDistanceCodesFactory
    {
        private readonly IGeneratingPolynomialsBuilder _generatingPolynomialsBuilder;
        private readonly IDecoder _decoder;
        private readonly IListDecoder _listDecoder;

        public StandardCodesFactory(IGeneratingPolynomialsBuilder generatingPolynomialsBuilder, IDecoder decoder, IListDecoder listDecoder)
        {
            if(generatingPolynomialsBuilder == null)
                throw new ArgumentNullException(nameof(generatingPolynomialsBuilder));
            if(decoder == null)
                throw new ArgumentNullException(nameof(decoder));
            if(listDecoder == null)
                throw new ArgumentNullException(nameof(listDecoder));

            _generatingPolynomialsBuilder = generatingPolynomialsBuilder;
            _decoder = decoder;
            _listDecoder = listDecoder;
        }

        public ICode Create(Polynomial generatingPolynomial)
        {
            if(generatingPolynomial == null)
                throw new ArgumentNullException(nameof(generatingPolynomial));

            var codewordLength = generatingPolynomial.Field.Order - 1;
            var informationWordLength = codewordLength % 2 == 0 ? codewordLength / 2 : (codewordLength - 1) / 2;

            var codeDistance = 0;
            var field = generatingPolynomial.Field;
            for (var i = 0; i < codewordLength; i++)
            {
                var zerosCount = 0;
                for (; i < codewordLength && generatingPolynomial.Evaluate(field.PowGeneratingElement(i)) == 0; i++, zerosCount++) ;
                codeDistance = Math.Max(codeDistance, zerosCount + 1);
            }
            if (codeDistance <= 1
                || codewordLength % 2 == 0 && codeDistance > codewordLength / 2 + 1
                || codewordLength % 2 == 1 && codeDistance > (codewordLength + 1) / 2 + 1)
                throw new InvalidOperationException("Generating polynomial is incorrect");

            return new FixedDistanceWaveletCode(
                codewordLength,
                informationWordLength,
                codeDistance,
                generatingPolynomial,
                _decoder,
                _listDecoder
            );
        }

        public ICode Create(Polynomial h, int requiredCodeDistance)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));

            var codewordLength = h.Field.Order - 1;
            var informationWordLength = codewordLength % 2 == 0 ? codewordLength / 2 : (codewordLength - 1) / 2;
            var generatingPolynomial = _generatingPolynomialsBuilder.Build(codewordLength, requiredCodeDistance, h);

            return new FixedDistanceWaveletCode(
                codewordLength,
                informationWordLength,
                requiredCodeDistance,
                generatingPolynomial,
                _decoder,
                _listDecoder
            );
        }
    }
}