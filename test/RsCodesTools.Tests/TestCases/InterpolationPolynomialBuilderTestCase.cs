namespace AppliedAlgebra.RsCodesTools.Tests.TestCases
{
    using System;
    using GfPolynoms;

    public class InterpolationPolynomialBuilderTestCase
    {
        public Tuple<int, int> DegreeWeight { get; set; }

        public int MaxWeightedDegree { get; set; }


        public Tuple<FieldElement, FieldElement>[] Roots { get; set; }
    }
}