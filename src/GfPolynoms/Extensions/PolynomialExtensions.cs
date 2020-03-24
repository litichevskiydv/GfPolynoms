namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
    using System.Linq;
    using GaloisFields;

    /// <summary>
    /// Class with extensions for polynomial
    /// </summary>
    public static class PolynomialExtensions
    {
        /// <summary>
        /// Method for performing replacing x->x^<paramref name="variableDegree"/>
        /// </summary>
        /// <param name="polynomial">Transformed polynomial</param>
        /// <param name="variableDegree">New variable degree</param>
        /// <returns>Replacement result</returns>
        public static Polynomial RaiseVariableDegree(this Polynomial polynomial, int variableDegree)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (variableDegree < 1)
                throw new ArgumentException(nameof(variableDegree));

            if (variableDegree == 1)
                return new Polynomial(polynomial);

            var resultCoefficients = new int[polynomial.Degree*variableDegree + 1];
            for (var i = 0; i <= polynomial.Degree; i++)
                resultCoefficients[i*variableDegree] = polynomial[i];
            return new Polynomial(polynomial.Field, resultCoefficients);
        }

        /// <summary>
        /// Method for obtaining polynomial coefficients
        /// </summary>
        public static FieldElement[] GetCoefficients(this Polynomial polynomial, int? expectedDegree = null)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if(expectedDegree.HasValue && expectedDegree.Value < 0)
                throw new ArgumentException($"{nameof(expectedDegree)} must be not negative");
            if(expectedDegree.HasValue && expectedDegree.Value < polynomial.Degree)
                throw new ArgumentException($"{nameof(expectedDegree)} must be greater or equal polynomial's degree");

            var coefficients = Enumerable.Repeat(polynomial.Field.Zero(), (expectedDegree ?? polynomial.Degree) + 1).ToArray();
            for (var i = 0; i <= polynomial.Degree; i++)
                coefficients[i] = polynomial.Field.CreateElement(polynomial[i]);

            return coefficients;
        }

        /// <summary>
        /// Method calculates <paramref name="polynomial"/> spectrum
        /// </summary>
        public static FieldElement[] GetSpectrum(this Polynomial polynomial, int? expectedDegree = null)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (expectedDegree.HasValue && expectedDegree.Value < 0)
                throw new ArgumentException($"{nameof(expectedDegree)} must be not negative");
            if (expectedDegree.HasValue && expectedDegree.Value < polynomial.Degree)
                throw new ArgumentException($"{nameof(expectedDegree)} must be greater or equal polynomial's degree");

            var coefficientsCount = (expectedDegree ?? polynomial.Degree) + 1;
            var fieldExtension = polynomial.Field.FindExtensionContainingPrimitiveRoot(coefficientsCount);
            var primitiveRoot = fieldExtension.GetPrimitiveRoot(coefficientsCount);
            var transferredPolynomial = polynomial.TransferToSubfield(fieldExtension);

            return Enumerable.Range(0, coefficientsCount)
                .Select(x => fieldExtension.CreateElement(transferredPolynomial.Evaluate(FieldElement.Pow(primitiveRoot, x).Representation)))
                .ToArray();
        }

        /// <summary>
        /// Transfers polynomial <paramref name="polynomial"/> to the compatible subfield
        /// of the field extension <paramref name="fieldExtension"/>
        /// </summary>
        /// <param name="polynomial">Transferred polynomial</param>
        /// <param name="fieldExtension">The field extension containing compatible subfield</param>
        public static Polynomial TransferToSubfield(this Polynomial polynomial, GaloisField fieldExtension)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (fieldExtension == null)
                throw new ArgumentNullException(nameof(fieldExtension));

            return new Polynomial(
                fieldExtension,
                Enumerable.Range(0, polynomial.Degree + 1)
                    .Select(i => polynomial.Field.TransferElementToSubfield(polynomial[i], fieldExtension))
                    .ToArray()
            );
        }

        /// <summary>
        /// Transfers polynomial <paramref name="polynomial"/> from subfield
        /// to the field <paramref name="field"/>
        /// </summary>
        /// <param name="polynomial">Transferred polynomial</param>
        /// <param name="field">Destination field</param>
        public static Polynomial TransferFromSubfield(this Polynomial polynomial, GaloisField field)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return new Polynomial(
                field,
                Enumerable.Range(0, polynomial.Degree + 1)
                    .Select(i => polynomial.Field.TransferElementFromSubfield(polynomial[i], field))
                    .ToArray()
            );
        }
    }
}