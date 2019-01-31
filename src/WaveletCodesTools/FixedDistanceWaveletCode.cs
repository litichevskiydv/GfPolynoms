namespace AppliedAlgebra.WaveletCodesTools
{
    using System.Collections.Generic;
    using System.Linq;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class FixedDistanceWaveletCode : ICode
    {
        private readonly Polynomial _generatingPolynomial;
        private readonly Polynomial _modularPolynomial;
        private readonly FieldElement[] _preparedPoints;

        public GaloisField Field { get; }
        public int CodewordLength { get; }
        public int InformationWordLength { get; }
        public int CodeDistance { get; }

        public FixedDistanceWaveletCode(
            int codewordLength, 
            int informationWordLength, 
            int codeDistance, 
            Polynomial generatingPolynomial)
        {
            Field = generatingPolynomial.Field;
            CodewordLength = codewordLength;
            InformationWordLength = informationWordLength;
            CodeDistance = codeDistance;

            _generatingPolynomial = generatingPolynomial;
            _modularPolynomial = new Polynomial(Field, 1).RightShift(CodewordLength) + new Polynomial(Field, Field.InverseForAddition(1));
            _preparedPoints = Enumerable.Range(0, CodeDistance)
                .Select(x => Field.CreateElement(Field.GetGeneratingElementPower(x)))
                .ToArray();
        }

        public FieldElement[] Encode(FieldElement[] informationWord)
        {
            var informationPolynomial = new Polynomial(informationWord);
            var codePolynomial = informationPolynomial.RaiseVariableDegree(2) * _generatingPolynomial % _modularPolynomial;
            return codePolynomial.GetCoefficients(CodewordLength - 1);
        }

        public FieldElement[] Decode(FieldElement[] noisyCodeword)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
        {
            throw new System.NotImplementedException();
        }
    }
}