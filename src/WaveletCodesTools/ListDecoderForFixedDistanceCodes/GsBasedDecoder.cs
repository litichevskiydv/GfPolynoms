namespace WaveletCodesTools.ListDecoderForFixedDistanceCodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class GsBasedDecoder : IListDecoder
    {
        private readonly RsCodesTools.ListDecoder.IListDecoder _rsListDecoder;
        private readonly ILinearSystemSolver _linearSystemSolver;

        private IEnumerable<Polynomial> GetFrequencyDecodingResults(int n, int d,
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword, int minCorrectValuesCount)
        {
            var field = decodedCodeword[0].Item1.Field;

            var correction = new FieldElement(field, n);
            var preparedCodeword = new Tuple<FieldElement, FieldElement>[n];
            for (var i = 0; i < n; i++)
            {
                var inversedSample = new FieldElement(field, field.InverseForMultiplication(decodedCodeword[i].Item1.Representation));
                preparedCodeword[i] = new Tuple<FieldElement, FieldElement>(inversedSample,
                    decodedCodeword[i].Item2*correction*FieldElement.Pow(decodedCodeword[i].Item1, d - 1));
            }
            return _rsListDecoder.Decode(n, n - d + 1, preparedCodeword, minCorrectValuesCount);
        }

        private Polynomial[] SelectCorrectInformationPolynomials(int n, int k, int d, Polynomial generatingPolynomial,
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword, IEnumerable<Polynomial> frequencyDecodingResults)
        {
            var correctPolynomials = new List<Polynomial>();

            var field = generatingPolynomial.Field;
            foreach (var frequencyDecodingResult in frequencyDecodingResults)
            {
                var linearSystemMatrix = new FieldElement[n, k];
                for (var i = 0; i < n; i++)
                {
                    var correction = new FieldElement(field, generatingPolynomial.Evaluate(decodedCodeword[i].Item1.Representation));
                    var samplePower = field.One();
                    var sampleSqr = decodedCodeword[i].Item1*decodedCodeword[i].Item1;
                    for (var j = 0; j < k; j++)
                    {
                        linearSystemMatrix[i, j] = samplePower*correction;
                        samplePower *= sampleSqr;
                    }
                }

                var valuesVector = new FieldElement[n];
                for (var i = 0; i < n; i++)
                    valuesVector[i] = field.Zero();
                for (int i = d - 1, j = 0; i < n && j <= frequencyDecodingResult.Degree; i++,j++)
                    valuesVector[i] = new FieldElement(field, frequencyDecodingResult[j]);

                var systemSolution = _linearSystemSolver.Solve(linearSystemMatrix, valuesVector);
                if (systemSolution.IsEmpty == false)
                    correctPolynomials.Add(new Polynomial(field, systemSolution.Solution.Select(x => x.Representation).ToArray()));
            }

            return correctPolynomials.ToArray();
        }

        public Polynomial[] Decode(int n, int k, int d, Polynomial generatingPolynomial, 
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount)
        {
            if (n <= 0)
                throw new ArgumentException(nameof(n));
            if (k <= 0 || k >= n)
                throw new ArithmeticException(nameof(k));
            if(generatingPolynomial == null)
                throw new ArgumentNullException(nameof(generatingPolynomial));
            if (decodedCodeword == null)
                throw new ArgumentNullException(nameof(decodedCodeword));
            if (decodedCodeword.Length != n)
                throw new AggregateException(nameof(decodedCodeword));

            var frequencyDecodingResults = GetFrequencyDecodingResults(n, d, decodedCodeword, minCorrectValuesCount);

            return SelectCorrectInformationPolynomials(n, k, d, generatingPolynomial, decodedCodeword, frequencyDecodingResults);
        }

        public GsBasedDecoder(RsCodesTools.ListDecoder.IListDecoder rsListDecoder, ILinearSystemSolver linearSystemSolver)
        {
            if (rsListDecoder == null)
                throw new ArgumentNullException(nameof(rsListDecoder));
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _rsListDecoder = rsListDecoder;
            _linearSystemSolver = linearSystemSolver;
        }
    }
}