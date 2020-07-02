namespace AppliedAlgebra.RsCodesTools.Tests.TestCases
{
    using System;
    using GfPolynoms;

    public class InterpolationPolynomialBuilderTestCase
    {
        public (int xWeight, int yWeight) DegreeWeight { get; set; }

        public int MaxWeightedDegree { get; set; }


        public (FieldElement xValue, FieldElement yValue)[] Roots { get; set; }
    }
}