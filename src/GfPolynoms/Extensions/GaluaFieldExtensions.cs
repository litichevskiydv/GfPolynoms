namespace AppliedAlgebra.GfPolynoms.Extensions
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

        /// <summary>
        /// Returns new field element constructed from representation
        /// </summary>
        public static FieldElement CreateElement(this GaloisField field, int representation)
        {
            return new FieldElement(field, representation);
        }
    }
}