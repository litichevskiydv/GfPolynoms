namespace AppliedAlgebra.RsCodesTools.CodesFactory
{
    using System;
    using CodesAbstractions;
    using Decoding.ListDecoder;
    using Decoding.StandartDecoder;
    using GfPolynoms.GaloisFields;

    public class StandardCodesFactory : ICodesFactory
    {
        private readonly IDecoder _decoder;
        private readonly IListDecoder _listDecoder;

        public ICode Create(GaloisField field, int codewordLength, int informationWordLength)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if(codewordLength <= 0)
                throw new ArgumentException($"{nameof(codewordLength)} must be positive");
            if (codewordLength > field.Order)
                throw new ArgumentException($"{nameof(codewordLength)} must not be greater than field order");
            if (informationWordLength <= 0)
                throw new ArgumentException($"{nameof(informationWordLength)} must be positive");
            if(informationWordLength >= codewordLength)
                throw new ArgumentException($"{nameof(informationWordLength)} must not be greater than codeword length");

            return new StandardReedSolomonCode(field, codewordLength, informationWordLength, _decoder, _listDecoder);
        }

        public StandardCodesFactory(IDecoder decoder, IListDecoder listDecoder)
        {
            if(decoder == null)
                throw new ArgumentNullException(nameof(decoder));
            if(listDecoder == null)
                throw new ArgumentNullException(nameof(listDecoder));

            _decoder = decoder;
            _listDecoder = listDecoder;
        }
    }
}