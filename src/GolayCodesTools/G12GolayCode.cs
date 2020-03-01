namespace AppliedAlgebra.GolayCodesTools
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class G12GolayCode : GolayCodeBase
    {
        public G12GolayCode() : base(12, 6, 6, new Polynomial(GaloisField.Create(3), 2, 0, 1, 2, 1, 1))
        {
        }
    }
}