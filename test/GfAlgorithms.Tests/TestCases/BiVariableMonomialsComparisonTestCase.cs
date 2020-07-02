namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases
{
    using System;

    public class BiVariableMonomialsComparisonTestCase
    {
        public (int xDegree, int yDegree) FirstOperand { get; set; }

        public (int xDegree, int yDegree) SecondOperand { get; set; }

        public int ExpectedResult { get; set; }
    }
}