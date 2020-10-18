namespace AppliedAlgebra.WaveletCodesTools
{
    using System;
    using CodesAbstractions;
    using Encoding;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class MultilevelWaveletCode : BruteForceCodeBase
    {
        private readonly IMultilevelEncoder _encoder;

        protected override FieldElement[] GenerateCodeword(FieldElement[] informationWord) =>
            _encoder.Encode(CodewordLength, informationWord);

        public MultilevelWaveletCode(
            IMultilevelEncoder encoder,
            GaloisField field,
            int codewordLength,
            int informationWordLength,
            int codeDistance
        )
            : base(field, codewordLength, informationWordLength, codeDistance)
        {
            if(encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            _encoder = encoder;
            GenerateCodewords(new int[InformationWordLength], 0);
        }

        public override string ToString() => $"W[{CodewordLength},{InformationWordLength},{CodeDistance}]";
    }
}