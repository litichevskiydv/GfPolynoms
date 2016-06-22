namespace RsListDecoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;
    using GfPolynoms;

    public interface IInterpolationPolynomialBuilder
    {
        BiVariablePolynomial Build(Tuple<int, int> degreeWeight, int maxWeightedDegree, Tuple<FieldElement, FieldElement>[] roots, int rootsMultiplicity);
    }
}