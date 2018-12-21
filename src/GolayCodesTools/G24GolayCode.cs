namespace AppliedAlgebra.GolayCodesTools
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class G24GolayCode : GolayCodeBase
    {
        public G24GolayCode() : base(
            24,
            12,
            8,
            new Polynomial(new PrimeOrderField(2), 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1)
        )
        {
        }
    }
}