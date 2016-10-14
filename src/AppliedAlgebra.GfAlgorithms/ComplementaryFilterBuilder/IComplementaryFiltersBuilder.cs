namespace AppliedAlgebra.GfAlgorithms.ComplementaryFilterBuilder
{
    using GfPolynoms;

    public interface IComplementaryFiltersBuilder
    {
        Polynomial Build(Polynomial sourceFilter, int maxFilterLength);
    }
}