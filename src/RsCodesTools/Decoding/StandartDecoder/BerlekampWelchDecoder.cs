namespace AppliedAlgebra.RsCodesTools.Decoding.StandartDecoder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of Reed-Solomon codes decoder based on Berlekamp-Welch algorithm
    /// </summary>
    public class BerlekampWelchDecoder : IDecoder
    {
        /// <summary>
        /// Implementation of linear equations system solver contract
        /// </summary>
        private readonly ILinearSystemSolver _linearSystemSolver;

        /// <summary>
        /// Method for generating a system of linear equations over the coefficients of the polynomials Q and E
        /// </summary>
        /// <param name="k">Information word length</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="errorsCount">Errors count</param>
        /// <returns>Generated system</returns>
        private static Tuple<FieldElement[,], FieldElement[]> BuildEquationsSystem(
            int k, 
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword,
            int errorsCount)
        {
            var a = new FieldElement[decodedCodeword.Count, k + 2 * errorsCount];
            var b = new FieldElement[decodedCodeword.Count];

            for (var i = 0; i < decodedCodeword.Count; i++)
            {
                var j = 0;
                var coefficient = decodedCodeword[i].Item1.Field.One();
                for (; j < k + errorsCount; j++)
                {
                    a[i, j] = coefficient;
                    coefficient *= decodedCodeword[i].Item1;
                }

                coefficient = FieldElement.InverseForAddition(decodedCodeword[i].Item2);
                for (; j < k + 2 * errorsCount; j++)
                {
                    a[i, j] = coefficient;
                    coefficient *= decodedCodeword[i].Item1;
                }
                b[i] = FieldElement.InverseForAddition(coefficient);
            }

            return Tuple.Create(a, b);
        }

        /// <summary>
        /// Method for computing information polynomial
        /// </summary>
        /// <param name="k">Information word length</param>
        /// <param name="errorsCount">Errors count</param>
        /// <param name="equationsSystem">Equations system</param>
        /// <returns>Computed information polynomial</returns>
        private Polynomial ComputeInformationPolynomial(int k, int errorsCount, Tuple<FieldElement[,], FieldElement[]> equationsSystem)
        {
            var solution = _linearSystemSolver.Solve(equationsSystem.Item1, equationsSystem.Item2);
            if(solution.IsEmpty)
                throw new InformationPolynomialWasNotFoundException();

            var field = solution.VariablesValues[0].Field;

            var q = new Polynomial(field,
                solution.VariablesValues
                    .Take(k + errorsCount)
                    .Select(x => x.Representation)
                    .ToArray());
            var e = new Polynomial(field,
                solution.VariablesValues
                    .Skip(k + errorsCount)
                    .Select(x => x.Representation)
                    .Concat(new[] {1})
                    .ToArray());
            return q / e;
        }

        /// <summary>
        /// Method for performing decoding of Reed-Solomon code codeword, if unsuccess will throw InformationPolynomialWasNotFoundException
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="errorsCount">Errors count</param>
        /// <returns>Decoding result</returns>
        public Polynomial Decode(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int? errorsCount = null)
        {
            var actualErrorsCount = errorsCount ?? (n - k) / 2;
            if (actualErrorsCount > (n - k) / 2)
                throw new InvalidOperationException("Errors count is too high");

            var equationsSystem = BuildEquationsSystem(k, decodedCodeword, actualErrorsCount);
            return ComputeInformationPolynomial(k, actualErrorsCount, equationsSystem);
        }

        /// <summary>
        /// Constructor for creation instance of decoder
        /// </summary>
        /// <param name="linearSystemSolver">Implementation of linear equations system solver contract</param>
        public BerlekampWelchDecoder(ILinearSystemSolver linearSystemSolver)
        {
            if(linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _linearSystemSolver = linearSystemSolver;
        }
    }
}