namespace AppliedAlgebra.CodesAbstractions
{
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Contract for code implementation
    /// </summary>
    public interface ICode
    {
        int CodewordLength { get; }
        int InformationWordLength { get; }
        int CodeDistance { get; }

        FieldElement[] Encode(FieldElement[] informationWord);

        FieldElement[] Decode(FieldElement[] codeword);

        IReadOnlyCollection<FieldElement[]> DecodeViaList(FieldElement[] codeword);
    }
}