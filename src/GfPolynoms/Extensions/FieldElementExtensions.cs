namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
    using GaloisFields;

    public static class FieldElementExtensions
    {
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
    }
}