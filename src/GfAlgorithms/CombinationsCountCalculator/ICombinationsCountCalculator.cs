namespace GfAlgorithms.CombinationsCountCalculator
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public interface ICombinationsCountCalculator
    {
        FieldElement Calculate(GaloisField field, int n, int k, IDictionary<Tuple<int, int>, FieldElement> combinationsCache = null);
    }
}