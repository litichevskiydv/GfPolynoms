namespace AppliedAlgebra.GfAlgorithms.Matrices
{
    using System;

    public class DiagonalizationOptions
    {
        public int MaxDegreeOfParallelism { get; }

        public DiagonalizationOptions()
        {
            MaxDegreeOfParallelism = -1;
        }
    }
}