namespace GfAlgorithms.CombinationsCountCalculator
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public interface ICombinationsCountCalculator
    {
        FieldElement Calculate(GaloisField field, int n, int k, FieldElement[][] combinationsCache = null);
    }
}