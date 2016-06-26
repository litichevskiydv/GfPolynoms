namespace WaveletCodesTools.GeneratingPolynomialsBuilder
{
    using System;
    using System.Linq;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class LiftingSchemeBasedBuilder : IGeneratingPolynomialsBuilder
    {
        private readonly IComplementaryFiltersBuilder _complementaryFiltersBuilder;
        private readonly ILinearSystemSolver _linearSystemSolver;

        private Polynomial FindLiftingPolynomial(int n, int d, Polynomial h, Polynomial g)
        {
            var field = h.Field;

            var linearSystemMatrix = new FieldElement[n/2, n/2];
            for (var i = 0; i < n/2; i++)
            {
                var sample = new FieldElement(field, field.GetGeneratingElementPower(n/2 + i));
                var sampleSqr = sample * sample;
                var samplePower = field.One();
                var correction = sampleSqr*new FieldElement(field, h.Evaluate(sample.Representation));
                for (var j = 0; j < n/2; j++)
                {
                    linearSystemMatrix[i, j] = samplePower*correction;
                    samplePower *= sampleSqr;
                }
            }

            var valuesVector = new FieldElement[n/2];
            for (var i = 0; i < n/2; i++)
            {
                var sample = new FieldElement(field, field.GetGeneratingElementPower(n / 2 + i));
                valuesVector[i] = new FieldElement(field, field.InverseForAddition(h.Evaluate(sample.Representation)))
                                  + sample*sample*new FieldElement(field, field.InverseForAddition(g.Evaluate(sample.Representation)));
            }
            for (var i = 0; i < n/2 - d + 1; i++)
                valuesVector[i] += field.One();

            var systemSolution = _linearSystemSolver.Solve(linearSystemMatrix, valuesVector);
            return new Polynomial(field, systemSolution.Solution.Select(x => x.Representation).ToArray());
        }

        private static Polynomial ReconstructGeneratingPolynomial(Polynomial h, Polynomial g, Polynomial liftingPolynomial, int maxFilterLength)
        {
            var hPolyphaseComponents = h.GetPolyphaseComponents();
            var gPolyphaseComponents = g.GetPolyphaseComponents();
            var m = new Polynomial(h.Field, 1).RightShift(maxFilterLength/2);
            m[0] = h.Field.InverseForAddition(1);

            return h +
                   PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(
                       hPolyphaseComponents.Item1*liftingPolynomial%m + gPolyphaseComponents.Item1,
                       hPolyphaseComponents.Item2*liftingPolynomial%m + gPolyphaseComponents.Item2)
                       .RightShift(2);
        }

        private static Polynomial CheckGeneratigPolynomial(int n, int d, Polynomial generatingPolynomial)
        {
            var field = generatingPolynomial.Field;

            var zeroValuesCount = 0;
            var sequentalNonzeroValuesCount = 0;
            var firstNonzeroValueMet = false;
            var lastNonzeroValueMet = false;
            for (var i = 0; i < n; i++)
            {
                if (generatingPolynomial.Evaluate(field.GetGeneratingElementPower(i)) == 0)
                {
                    if (firstNonzeroValueMet)
                        lastNonzeroValueMet = true;
                    zeroValuesCount++;
                }
                else
                {
                    if (firstNonzeroValueMet == false)
                        firstNonzeroValueMet = true;
                    if (lastNonzeroValueMet == false)
                        sequentalNonzeroValuesCount++;
                }
            }

            if (zeroValuesCount != d - 1 || sequentalNonzeroValuesCount != n - d + 1)
                throw new InvalidOperationException("Generating polynomial is incorrect");
            return generatingPolynomial;
        }

        public Polynomial Build(int n, int d, Polynomial sourceFilter)
        {
            if (n <= 0)
                throw new ArgumentException(nameof(n));
            if (d <= 0 || d > n/2 + 1)
                throw new ArgumentException(nameof(d));
            if (sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));

            var complementaryFilter = _complementaryFiltersBuilder.Build(sourceFilter, n);
            var liftingPolynomial = FindLiftingPolynomial(n, d, sourceFilter, complementaryFilter);
            var generatingPolynomial = ReconstructGeneratingPolynomial(sourceFilter, complementaryFilter, liftingPolynomial, n);
            return CheckGeneratigPolynomial(n, d, generatingPolynomial);
        }

        public LiftingSchemeBasedBuilder(IComplementaryFiltersBuilder complementaryFiltersBuilder, ILinearSystemSolver linearSystemSolver)
        {
            if(complementaryFiltersBuilder == null)
                throw new ArgumentNullException(nameof(complementaryFiltersBuilder));
            if(linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _complementaryFiltersBuilder = complementaryFiltersBuilder;
            _linearSystemSolver = linearSystemSolver;
        }
    }
}