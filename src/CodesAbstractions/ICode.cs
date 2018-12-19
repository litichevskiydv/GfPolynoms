namespace AppliedAlgebra.CodesAbstractions
{
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Contract for code implementation
    /// </summary>
    public interface ICode
    {
        GaloisField Field { get; }

        int CodewordLength { get; }
        int InformationWordLength { get; }
        int CodeDistance { get; }

        FieldElement[] Encode(FieldElement[] informationWord);

        FieldElement[] Decode(FieldElement[] noisyCodeword);

        IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null);
    }
}