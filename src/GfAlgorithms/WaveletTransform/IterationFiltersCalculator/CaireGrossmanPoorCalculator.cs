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
    public class CaireGrossmanPoorCalculator : IterationFiltersCalculatorBase
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

        protected override FieldElement[] GetIterationFilter(
            int iterationNumber,
            GaloisField field,
            FieldElement[] sourceFilter,
            int sourceFilterLength,
            int filterLengthDecreasesTimes
        )
        {
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
    }
}