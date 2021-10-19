namespace AppliedAlgebra.WaveletCodesTools.GeneratingPolynomialsBuilder
{
    using System;
    using System.Linq;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of contract of generating polynomials builder
    /// </summary>
    public class LiftingSchemeBasedBuilder : IGeneratingPolynomialsBuilder
    {
        /// <summary>
        /// Implementation of contract of complementary filters builder
        /// </summary>
        private readonly IComplementaryFiltersBuilder _complementaryFiltersBuilder;
        /// <summary>
        /// Implementation of contract of linear equations system solver
        /// </summary>
        private readonly ILinearSystemSolver _linearSystemSolver;

        /// <summary>
        /// Implementation of contract of generating polynomials factory
        /// </summary>
        private readonly IGeneratingPolynomialsFactory _generatingPolynomialsFactory;

        /// <summary>
        /// Method for finding lifting polynomial for filters pair (<paramref name="h"/>, <paramref name="g"/>)
        /// </summary>
        /// <param name="h">First filter from the original pair</param>
        /// <param name="g">Second filter from the original pair</param>
        /// <param name="equationsCount">Equations count</param>
        /// <param name="variablesCount">Variables count</param>
        /// <returns>Found lifting polynomial</returns>
        private Polynomial FindLiftingPolynomial(Polynomial h, Polynomial g, int equationsCount, int variablesCount)
        {
            var field = h.Field;

            var linearSystemMatrix = new FieldElement[variablesCount, variablesCount];
            for (var i = 0; i < linearSystemMatrix.GetLength(0); i++)
            {
                var sample = new FieldElement(field, field.PowGeneratingElement(i));
                var sampleSqr = sample * sample;
                var samplePower = field.One();
                var correction = sampleSqr*new FieldElement(field, h.Evaluate(sample.Representation));
                for (var j = 0; j < linearSystemMatrix.GetLength(1); j++)
                {
                    linearSystemMatrix[i, j] = samplePower*correction;
                    samplePower *= sampleSqr;
                }
            }

            var valuesVector = new FieldElement[variablesCount];
            for (var i = 0; i < valuesVector.Length; i++)
            {
                var sample = new FieldElement(field, field.PowGeneratingElement(i));
                valuesVector[i] = new FieldElement(field, field.InverseForAddition(h.Evaluate(sample.Representation)))
                                  + sample * sample * new FieldElement(field, field.InverseForAddition(g.Evaluate(sample.Representation)));
            }
            for (var i = equationsCount; i < valuesVector.Length; i++)
                valuesVector[i] += field.One();

            var systemSolution = _linearSystemSolver.Solve(linearSystemMatrix, valuesVector);
            if(systemSolution.IsEmpty)
                throw new InvalidOperationException("Can't find lifting polynomial");
            return new Polynomial(field, systemSolution.VariablesValues.Select(x => x.Representation).ToArray());
        }

        /// <summary>
        /// Method for performing generating polynomial reconstruction from filters pair (<paramref name="h"/>, <paramref name="g"/>) and lifting polynomial <paramref name="liftingPolynomial"/>
        /// </summary>
        /// <param name="h">First filter from the original pair</param>
        /// <param name="g">Second filter from the original pair</param>
        /// <param name="liftingPolynomial">Lifting polynomial</param>
        /// <param name="n">Maximum possible original filter length</param>
        /// <returns>Reconstructed generating polynomial</returns>
        private Polynomial ReconstructGeneratingPolynomial(Polynomial h, Polynomial g, Polynomial liftingPolynomial, int n)
            => _generatingPolynomialsFactory.Create(h, g + h * liftingPolynomial.RaiseVariableDegree(2), n - 1);

        /// <summary>
        /// Method for verification of the generating polynomial 
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generatingPolynomial">Generating polynomial for verification</param>
        /// <returns>Verified generating polynomial</returns>
        private static Polynomial CheckGeneratingPolynomial(int n, int d, Polynomial generatingPolynomial)
        {
            var field = generatingPolynomial.Field;

            var zeroValuesCount = 0;
            var nonZeroValuesCount = 0;

            var i = 0;
            var j = n - 1;
            for (; i < n && generatingPolynomial.Evaluate(field.PowGeneratingElement(i)) == 0; i++)
                zeroValuesCount++;
            for (; j > i && generatingPolynomial.Evaluate(field.PowGeneratingElement(j)) == 0; j--)
                zeroValuesCount++;
            for (; i <= j; i++)
                if (generatingPolynomial.Evaluate(field.PowGeneratingElement(i)) != 0)
                    nonZeroValuesCount++;

            if (zeroValuesCount < d - 1 || n % 2 == 0 && nonZeroValuesCount < n / 2 || n % 2 == 1 && nonZeroValuesCount < (n - 1) / 2)
                throw new InvalidOperationException("Generating polynomial is incorrect");
            return generatingPolynomial;
        }

        /// <summary>
        /// Method for building generating polynomial for the wavelet code
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="d">Code distance</param>
        /// <param name="h">Filter from which generating polynomial will be built</param>
        /// <returns>Built generating polynomial</returns>
        public Polynomial Build(int n, int d, Polynomial h)
        {
            if (n <= 0)
                throw new ArgumentException(nameof(n));
            if (d <= 1 || n % 2 == 0 && d > n / 2 + 1 || n % 2 == 1 && d > (n + 1) / 2)
                throw new ArgumentException(nameof(d));
            if (h == null)
                throw new ArgumentNullException(nameof(h));

            var g = _complementaryFiltersBuilder.Build(h, n);
            var liftingPolynomial = FindLiftingPolynomial(h, g, d - 1, n % 2 == 0 ? n / 2 : (n - 1) / 2);
            var generatingPolynomial = ReconstructGeneratingPolynomial(h, g, liftingPolynomial, n);
            return CheckGeneratingPolynomial(n, d, generatingPolynomial);
        }

        /// <summary>
        /// Constructor for creation implementation of contract of generating polynomials builder
        /// </summary>
        /// <param name="complementaryFiltersBuilder">Implementation of contract of complementary filters builder</param>
        /// <param name="linearSystemSolver">Implementation of contract of linear equations system solver</param>
        /// <param name="generatingPolynomialsFactory">Implementation of contract of generating polynomials factory</param>
        public LiftingSchemeBasedBuilder(
            IComplementaryFiltersBuilder complementaryFiltersBuilder,
            ILinearSystemSolver linearSystemSolver,
            IGeneratingPolynomialsFactory generatingPolynomialsFactory
        )
        {
            if (complementaryFiltersBuilder == null)
                throw new ArgumentNullException(nameof(complementaryFiltersBuilder));
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));
            if (generatingPolynomialsFactory == null)
                throw new ArgumentNullException(nameof(generatingPolynomialsFactory));

            _complementaryFiltersBuilder = complementaryFiltersBuilder;
            _linearSystemSolver = linearSystemSolver;
            _generatingPolynomialsFactory = generatingPolynomialsFactory;
        }
    }
}