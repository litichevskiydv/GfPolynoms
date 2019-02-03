namespace AppliedAlgebra.GolayCodesTools
{
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public abstract class GolayCodeBase : BruteForceCodeBase
    {
        private readonly Polynomial _generatingPolynomial;
        private readonly Polynomial _modularPolynomial;

        protected override FieldElement[] GenerateCodeword(FieldElement[] informationWord)
        {
            var informationPolynomial = new Polynomial(informationWord);
            var codePolynomial = informationPolynomial * _generatingPolynomial % _modularPolynomial;
            if (CodewordLength % 2 == 0)
                codePolynomial += new Polynomial(Field, informationPolynomial.Evaluate(1)).RightShift(CodewordLength - 1);

            return codePolynomial.GetCoefficients(CodewordLength - 1);
        }

        protected GolayCodeBase(int codewordLength, int informationWordLength, int codeDistance, Polynomial generatingPolynomial) 
            : base(generatingPolynomial.Field, codewordLength, informationWordLength, codeDistance)
        {
            _generatingPolynomial = generatingPolynomial;
            _modularPolynomial =
                new Polynomial(Field, 1).RightShift(CodewordLength - (CodewordLength % 2 == 0 ? 1 : 0))
                + new Polynomial(Field, Field.InverseForAddition(1));
            GenerateCodewords(new int[InformationWordLength], 0);
        }

        public override string ToString()
        {
            return $"G{CodewordLength}[{CodewordLength},{InformationWordLength},{CodeDistance}]";
        }
    }
}