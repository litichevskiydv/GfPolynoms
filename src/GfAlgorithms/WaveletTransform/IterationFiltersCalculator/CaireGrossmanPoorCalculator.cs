namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.IterationFiltersCalculator
{
    using System;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using PolynomialExtensions = Extensions.PolynomialExtensions;

    /// <summary>
    /// Iteration filters calculator based on Caire-Grossman-Poor algorithm
    /// </summary>
    public class CaireGrossmanPoorCalculator : IIterationFiltersCalculator
    {
        private static Polynomial GetIterationFilterComponent(
            Polynomial sourceFilterComponent,
            int iterationFilterComponentLength,
            FieldElement primitiveRoot,
            GaloisField field)
        {
            var fieldExtension = primitiveRoot.Field;
            var transferredSourceFilterComponent = sourceFilterComponent.TransferToSubfield(fieldExtension);

            var argument = fieldExtension.One();
            var iterationFilterComponentValues = new FieldElement[iterationFilterComponentLength];
            for (var i = 0; i < iterationFilterComponentLength; i++, argument *= primitiveRoot)
                iterationFilterComponentValues[i] = fieldExtension.CreateElement(transferredSourceFilterComponent.Evaluate(argument.Representation));

            return new Polynomial(iterationFilterComponentValues.GetSignal()).TransferFromSubfield(field);
        }

        /// <inheritdoc />
        public FieldElement[] GetIterationFilter(int iterationNumber, FieldElement[] sourceFilter)
        {
            if(iterationNumber <= 0)
                throw new ArgumentException($"{nameof(iterationNumber)} must be positive");
            if (sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));
            if(sourceFilter.Length == 0)
                throw new ArgumentException($"{nameof(sourceFilter)} length must be positive");

            if (iterationNumber == 1)
                return sourceFilter;

            var sourceFilterLength = sourceFilter.Length;
            var filterLengthDecreasesTimes = 2.Pow(iterationNumber - 1);
            if (sourceFilterLength % (2 * filterLengthDecreasesTimes) != 0)
                throw new ArgumentException($"{nameof(sourceFilter)} length is incorrect");

            var field = sourceFilter.GetField();
            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(sourceFilterLength / 2);
            var primitiveRoot = fieldExtension.GetPrimitiveRoot(sourceFilterLength / 2).Pow(filterLengthDecreasesTimes);

            var (sourceFilterEvenComponent, sourceFilterOddComponent) = new Polynomial(sourceFilter).GetPolyphaseComponents();
            var iterationFilterComponentLength = sourceFilterLength / (2 * filterLengthDecreasesTimes);
            return PolynomialExtensions.CreateFormPolyphaseComponents(
                    GetIterationFilterComponent(sourceFilterEvenComponent, iterationFilterComponentLength, primitiveRoot, field),
                    GetIterationFilterComponent(sourceFilterOddComponent, iterationFilterComponentLength, primitiveRoot, field)
                )
                .GetCoefficients(2 * iterationFilterComponentLength - 1);
        }

        /// <inheritdoc />
        public Polynomial GetIterationFilter(int iterationNumber, Polynomial sourceFilter, int? expectedDegree = null)
        {
            if(sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));

            return new Polynomial(GetIterationFilter(iterationNumber, sourceFilter.GetCoefficients(expectedDegree)));
        }
    }
}