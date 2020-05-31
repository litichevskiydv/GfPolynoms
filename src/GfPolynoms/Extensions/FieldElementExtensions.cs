namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
    using GaloisFields;

    public static class FieldElementExtensions
    {
        /// <summary>
        /// Finds minimal polynomial for field element <paramref name="element"/>
        /// </summary>
        /// <param name="element">Field element whose minimal polynomial is to be constructed</param>
        public static Polynomial FindMinimalPolynomial(this FieldElement element)
        {
            if(element == null)
                throw new ArgumentNullException(nameof(element));
            return element.Field.FindMinimalPolynomial(element.Representation);
        }

        /// <summary>
        /// Transfers element <paramref name="element"/> to the compatible subfield
        /// of the field extension <paramref name="fieldExtension"/>
        /// </summary>
        /// <param name="element">Transferred element</param>
        /// <param name="fieldExtension">The field extension containing compatible subfield</param>
        public static FieldElement TransferToSubfield(this FieldElement element, GaloisField fieldExtension)
        {
            if(element == null)
                throw new ArgumentNullException(nameof(element));
            if (fieldExtension == null)
                throw new ArgumentNullException(nameof(fieldExtension));

            return new FieldElement(fieldExtension, element.Field.TransferElementToSubfield(element.Representation, fieldExtension));
        }

        /// <summary>
        /// Transfers element <paramref name="subfieldElement"/> from subfield
        /// to the field <paramref name="field"/>
        /// </summary>
        /// <param name="subfieldElement">Transferred subfield element</param>
        /// <param name="field">Destination field</param>
        public static FieldElement TransferFromSubfield(this FieldElement subfieldElement, GaloisField field)
        {
            if (subfieldElement == null)
                throw new ArgumentNullException(nameof(subfieldElement));
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return new FieldElement(field, subfieldElement.Field.TransferElementFromSubfield(subfieldElement.Representation, field));
        }
    }
}