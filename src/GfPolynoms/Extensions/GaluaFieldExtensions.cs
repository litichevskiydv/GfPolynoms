namespace GfPolynoms.Extensions
{
    using GaluaFields;

    public static class GaluaFieldExtensions
    {
        /// <summary>
        /// Return zero element for specified field
        /// </summary>
        public static FieldElement Zero(this IGaluaField field)
        {
            return new FieldElement(field, 0);
        }

        /// <summary>
        /// Return zero element for specified field
        /// </summary>
        public static FieldElement One(this IGaluaField field)
        {
            return new FieldElement(field, 1);
        }
    }
}