namespace AppliedAlgebra.GfAlgorithms.PolynomialsFactorizer
{
    public class BerlekampFactorizerOptions
    {
        public int MaxDegreeOfParallelism { get; set; }

        public BerlekampFactorizerOptions()
        {
            MaxDegreeOfParallelism = -1;
        }
    }
}