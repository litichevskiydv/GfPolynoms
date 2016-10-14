namespace AppliedAlgebra.RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;
    using GfAlgorithms.BiVariablePolynomials;
    using GfPolynoms;

    public interface IInterpolationPolynomialBuilder
    {
        BiVariablePolynomial Build(Tuple<int, int> degreeWeight, int maxWeightedDegree, Tuple<FieldElement, FieldElement>[] roots, int rootsMultiplicity);
    }
}