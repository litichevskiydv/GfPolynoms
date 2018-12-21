namespace AppliedAlgebra.GolayCodesTools
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class G23GolayCode : GolayCodeBase
    {
        public G23GolayCode() : base(
            23,
            12,
            7,
            new Polynomial(new PrimeOrderField(2), 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1)
        )
        {
        }
    }
}