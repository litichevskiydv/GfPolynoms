namespace AppliedAlgebra.GfAlgorithms.VectorsIterator
{
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public interface IVariantsIterator
    {
        IEnumerable<FieldElement[]> IterateVectors(GaloisField field, int length);

        IEnumerable<Polynomial> IteratePolynomials(GaloisField field, int maxDegree);
    }
}