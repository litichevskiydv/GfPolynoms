namespace AppliedAlgebra.RsCodesTools
{
    using System.Linq;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class ReedSolomonCode : BruteForceCodeBase
    {
        public ReedSolomonCode(GaloisField field, int codewordLength, int informationWordLength) : base(field, codewordLength,
            informationWordLength, codewordLength - informationWordLength + 1)
        {
            GenerateCodewords(new int[InformationWordLength], 0);
        }

        protected override FieldElement[] GenerateCodeword(FieldElement[] informationWord)
        { 
            var informationPolynomial = new Polynomial(informationWord);
            return Enumerable.Range(0, CodewordLength)
                .Select(x => Field.CreateElement(informationPolynomial.Evaluate(Field.PowGeneratingElement(x))))
                .ToArray();
        }
        

        public override string ToString() => $"RS[{CodewordLength},{InformationWordLength}]";
    }
}