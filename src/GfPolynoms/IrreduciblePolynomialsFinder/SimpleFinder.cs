namespace GfPolynoms.IrreduciblePolynomialsFinder
{
    using System;
    using GaloisFields;

    public class SimpleFinder : IIrreduciblePolynomialsFinder
    {
        private static bool Find(Polynomial polynomial, int position)
        {
            if (polynomial.Degree == position)
            {
                for (var i = 0; i < polynomial.Field.Order; i++)
                    if (polynomial.Evaluate(i) == 0)
                        return false;
                return true;
            }

            var checkResult = false;
            var originalValue = polynomial[position];
            for (var i = originalValue; i < polynomial.Field.Order && checkResult == false; i++)
            {
                polynomial[position] = i;
                checkResult = Find(polynomial, position + 1);
            }

            if (checkResult == false)
                polynomial[position] = originalValue;
            return checkResult;
        }

        public Polynomial Find(int fieldOrder, int degree)
        {
            if (degree < 2)
                throw new ArgumentException(nameof(degree));

            var result = new Polynomial(new PrimeOrderField(fieldOrder), 1).RightShift(degree);
            result[0] = 1;

            if (Find(result, 0))
                return result;
            throw new InvalidOperationException("Irreducible Polynomial doesn't found");
        }
    }
}