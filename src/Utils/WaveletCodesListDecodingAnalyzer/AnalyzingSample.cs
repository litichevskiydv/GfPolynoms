namespace WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Linq;
    using GfPolynoms;

    public class AnalyzingSample
    {
        public Polynomial InformationPolynomial { get; }

        public Tuple<FieldElement, FieldElement>[] Codeword { get; }

        public AnalyzingSample(Polynomial informationPolynomial, Tuple<FieldElement, FieldElement>[] codeword)
        {
            InformationPolynomial = informationPolynomial;
            Codeword = codeword;
        }

        public AnalyzingSample(AnalyzingSample sample)
        {
            InformationPolynomial = sample.InformationPolynomial;
            Codeword = sample.Codeword.Select(x => new Tuple<FieldElement, FieldElement>(x.Item1, new FieldElement(x.Item2))).ToArray();
        }
    }
}