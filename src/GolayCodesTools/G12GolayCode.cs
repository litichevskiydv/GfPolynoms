namespace AppliedAlgebra.GolayCodesTools
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class G12GolayCode : GolayCodeBase
    {
        public G12GolayCode() : base(12, 6, 6, new Polynomial(new PrimeOrderField(3), 2, 2, 1, 2, 2, 2, 1, 1, 1, 2, 1))
        {
        }
    }
}