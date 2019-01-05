namespace AppliedAlgebra.GolayCodesTools
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class G11GolayCode : GolayCodeBase
    {
        public G11GolayCode() : base(11, 6, 5, new Polynomial(new PrimeOrderField(3), 2, 0, 1, 2, 1, 1))
        {
        }
    }
}