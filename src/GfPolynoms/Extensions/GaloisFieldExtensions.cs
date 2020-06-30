namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaloisFields;

    public static class GaloisFieldExtensions
    {
        /// <summary>
        /// Return zero element for specified field
        /// </summary>
        public static FieldElement Zero(this GaloisField field)
        {
            return new FieldElement(field, 0);
        }

        /// <summary>
        /// Return zero element for specified field
        /// </summary>
        public static FieldElement One(this GaloisField field)
        {
            return new FieldElement(field, 1);
        }

        /// <summary>
        /// Returns new field element constructed from representation
        /// </summary>
        public static FieldElement CreateElement(this GaloisField field, int representation)
        {
            return new FieldElement(field, representation);
        }

        /// <summary>
        /// Returns field elements vector constructed from components representation
        /// </summary>
        public static FieldElement[] CreateElementsVector(this GaloisField field, params int[] componentsRepresentations)
        {
            if(componentsRepresentations == null)
                throw new ArgumentNullException(nameof(componentsRepresentations));
            if(componentsRepresentations.Length == 0)
                throw new ArgumentException($"{componentsRepresentations.Length} must not be zero");

            return componentsRepresentations.Select(field.CreateElement).ToArray();
        }

        /// <summary>
        /// Finds extension of the field <paramref name="field"/> containing root of unity of the order <paramref name="rootOrder"/>
        /// </summary>
        /// <param name="field">Extendable field</param>
        /// <param name="rootOrder">Root order</param>
        public static GaloisField FindExtensionContainingPrimitiveRoot(this GaloisField field, int rootOrder)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if(rootOrder <= 0)
                throw new ArgumentException($"{nameof(rootOrder)} must be greater than zero");

            var fieldExtensionOrder = field.Order;
            for (; (fieldExtensionOrder - 1) % rootOrder != 0; fieldExtensionOrder *= field.Order) ;
            return GaloisField.Create(fieldExtensionOrder);
        }

        /// <summary>
        /// Returns root of unity of the order <paramref name="rootOrder"/> belonging to the field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field containing the root</param>
        /// <param name="rootOrder">Root order</param>
        public static FieldElement GetPrimitiveRoot(this GaloisField field, int rootOrder)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if (rootOrder <= 0)
                throw new ArgumentException($"{nameof(rootOrder)} must be greater than zero");
            if ((field.Order - 1) % rootOrder != 0)
                throw new ArgumentException($"Root with order {rootOrder} does not exists in the field {field}");

            return field.CreateElement(field.PowGeneratingElement((field.Order - 1) / rootOrder));
        }

        /// <summary>
        /// Generates conjugacy classes for field <paramref name="field"/> of the modulo <paramref name="modulus"/>
        /// </summary>
        /// <param name="field">Field whose order was used for classes generation</param>
        /// <param name="modulus">Modulo used for classes generation</param>
        public static int[][] GenerateConjugacyClasses(this GaloisField field, int modulus)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if(modulus < 1)
                throw new ArgumentException($"{nameof(modulus)} must be positive");

            var conjugacyClasses = new List<int[]>();
            var processed = new bool[modulus];
            for (var i = 0; i < modulus; i++)
            {
                if(processed[i]) continue;

                var conjugacyClass = new List<int>();
                for (var j = i; processed[j] == false; j = j * field.Order % modulus)
                {
                    processed[j] = true;
                    conjugacyClass.Add(j);
                }

                conjugacyClasses.Add(conjugacyClass.ToArray());
            }

            return conjugacyClasses.ToArray();
        }

        /// <summary>
        /// Generates minimal polynomial for field <paramref name="fieldExtension"/> element <paramref name="fieldElement"/>
        /// </summary>
        /// <param name="fieldExtension">The field containing element whose minimal polynomial is to be constructed</param>
        /// <param name="fieldElement">Field element whose minimal polynomial is to be constructed</param>
        public static Polynomial FindMinimalPolynomial(this GaloisField fieldExtension, int fieldElement)
        {
            if(fieldExtension == null)
                throw new ArgumentNullException(nameof(fieldExtension));

            var field = GaloisField.Create(fieldExtension.Characteristic);
            if (fieldElement == 0)
                return new Polynomial(field, 0, 1);

            return field.GenerateConjugacyClasses(fieldExtension.Order - 1)
                .Single(x => x.Contains(fieldExtension.GetGeneratingElementDegree(fieldElement)))
                .Aggregate(
                    new Polynomial(fieldExtension, 1),
                    (polynomial, classMember) =>
                        polynomial
                        * new Polynomial(
                            fieldExtension,
                            fieldExtension.InverseForAddition(fieldExtension.PowGeneratingElement(classMember)), 1
                        )
                )
                .TransferFromSubfield(field);
        }

        /// <summary>
        /// Transfers element <paramref name="fieldElement"/> of the field <paramref name="field"/>
        /// to the compatible subfield of the field extension <paramref name="fieldExtension"/>
        /// </summary>
        /// <param name="field">The field containing the transferred element</param>
        /// <param name="fieldElement">Transferred element</param>
        /// <param name="fieldExtension">The field extension containing compatible subfield</param>
        public static int TransferElementToSubfield(this GaloisField field, int fieldElement, GaloisField fieldExtension)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if(fieldExtension == null)
                throw new ArgumentNullException(nameof(fieldExtension));
            if (field.Characteristic != fieldExtension.Characteristic || (fieldExtension.Order - 1) % (field.Order - 1) != 0)
                throw new ArgumentException($"Field {fieldExtension} does not contains subfield {field}");

            var degreesDelta = (fieldExtension.Order - 1) / (field.Order - 1);
            return fieldElement == 0
                ? 0
                : fieldExtension.PowGeneratingElement(degreesDelta * field.GetGeneratingElementDegree(fieldElement));
        }

        /// <summary>
        /// Transfers element <paramref name="subfieldElement"/> from subfield <paramref name="field"/>
        /// of the field <paramref name="fieldExtension"/> to the field <paramref name="field"/>
        /// </summary>
        /// <param name="fieldExtension">The field extension containing compatible subfield</param>
        /// <param name="subfieldElement">Transferred subfield element</param>
        /// <param name="field">Destination field</param>
        public static int TransferElementFromSubfield(this GaloisField fieldExtension, int subfieldElement, GaloisField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (fieldExtension == null)
                throw new ArgumentNullException(nameof(fieldExtension));
            if (field.Characteristic != fieldExtension.Characteristic || (fieldExtension.Order - 1) % (field.Order - 1) != 0)
                throw new ArgumentException($"Field {fieldExtension} does not contains subfield {field}");

            if (subfieldElement == 0)
                return 0;

            var degreesDelta = (fieldExtension.Order - 1) / (field.Order - 1);
            var generatingElementPower = fieldExtension.GetGeneratingElementDegree(subfieldElement);
            if(generatingElementPower % degreesDelta != 0)
                throw new ArgumentException($"{subfieldElement} does not belong to subfield {field} of {fieldExtension}");

            return field.PowGeneratingElement(generatingElementPower / degreesDelta);
        }
    }
}