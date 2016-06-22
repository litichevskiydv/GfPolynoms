namespace GfPolynoms.Extensions
{
    using GaloisFields;

    public static class GaluaFieldExtensions
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
    }
}