namespace AppliedAlgebra.WaveletCodesTools
{
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class WaveletCode : BruteForceCodeBase
    {
        private readonly Polynomial _generatingPolynomial;
        private readonly Polynomial _modularPolynomial;

        protected override FieldElement[] GenerateCodeword(FieldElement[] informationWord) =>
            (new Polynomial(informationWord).RaiseVariableDegree(2) * _generatingPolynomial % _modularPolynomial)
            .GetCoefficients(CodewordLength - 1);

        public WaveletCode(int codewordLength, int informationWordLength, int codeDistance, Polynomial generatingPolynomial)
            : base(generatingPolynomial.Field, codewordLength, informationWordLength, codeDistance)
        {
            _generatingPolynomial = generatingPolynomial;
            _modularPolynomial = new Polynomial(Field, 1).RightShift(CodewordLength) + new Polynomial(Field, Field.InverseForAddition(1));
            GenerateCodewords(new int[InformationWordLength], 0);
        }

        public override string ToString() => $"W[{CodewordLength},{InformationWordLength},{CodeDistance}]";
    }
}