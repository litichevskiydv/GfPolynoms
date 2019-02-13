namespace AppliedAlgebra.WaveletCodesListDecodingAnalyzer
{
    using System.Linq;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class AnalyzingSample
    {
        public ICode Code { get; }
        public FieldElement[] InformationWord { get; }
        public FieldElement[] Codeword { get; }

        public AnalyzingSample(ICode code, FieldElement[] informationWord = null)
        {
            Code = code;
            InformationWord = informationWord ?? Enumerable.Repeat(Code.Field.Zero(), Code.InformationWordLength).ToArray();
            Codeword = code.Encode(InformationWord);
        }

        public int[] ErrorsPositions { get; set; }
        public int[] ErrorsValues { get; set; }
        public int CorrectValuesCount { get; set; }

        public int ProcessedNoises { get; set; }
    }
}