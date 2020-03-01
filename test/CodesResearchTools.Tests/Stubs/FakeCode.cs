namespace AppliedAlgebra.CodesResearchTools.Tests.Stubs
{
    using System.Collections.Generic;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class FakeCode : ICode
    {
        public GaloisField Field { get; } = GaloisField.Create(2);
        public int CodewordLength => 3;
        public int InformationWordLength => 2;
        public int CodeDistance => 0;

        public FieldElement[] Encode(FieldElement[] informationWord)
        {
            return new[] { informationWord[0], informationWord[1], informationWord[0] + informationWord[1] };
        }

        public FieldElement[] Decode(FieldElement[] noisyCodeword)
        {
            return new[] { Field.One(), Field.One() };
        }

        public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
        {
            var list = new List<FieldElement[]> { new[] { noisyCodeword[0], noisyCodeword[1] } };

            if (Equals(noisyCodeword[0] + noisyCodeword[1], noisyCodeword[2]) == false || listDecodingRadius == 2)
            {
                list.Add(new[] { noisyCodeword[0] + Field.One(), noisyCodeword[1] });
                list.Add(new[] { noisyCodeword[0], noisyCodeword[1] + Field.One() });
            }

            if (Equals(noisyCodeword[0] + noisyCodeword[1], noisyCodeword[2]) && listDecodingRadius == 2)
                list.Add(new[] { noisyCodeword[0] + Field.One(), noisyCodeword[1] + Field.One() });


            return list;
        }
    }
}