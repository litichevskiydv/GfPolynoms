namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
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

            return field.CreateElement(field.GetGeneratingElementPower((field.Order - 1) / rootOrder));
        }
    }
}