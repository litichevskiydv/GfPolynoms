namespace AppliedAlgebra.GolayCodesTools
{
    using GfPolynoms.GaloisFields;

    public class G12GolayCode : GolayCodeBase
    {
        public G12GolayCode() : base(new PrimeOrderField(3), 12, 6, 6)
        {
        }
    }
}