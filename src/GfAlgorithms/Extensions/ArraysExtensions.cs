namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    /// <summary>
    /// Class for arrays extensions
    /// </summary>
    public static class ArraysExtensions
    {
        /// <summary>
        /// Method for initializing rectangular array
        /// </summary>
        /// <typeparam name="TElement">Type of array elements</typeparam>
        /// <param name="array">Initialised array</param>
        /// <param name="secondDimensionLength">Array's second dimension length</param>
        public static TElement[][] MakeRectangle<TElement>(this TElement[][] array, int secondDimensionLength)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = new TElement[secondDimensionLength];
            return array;
        }

        /// <summary>
        /// Method for initializing square array
        /// </summary>
        /// <typeparam name="TElement">Type of array elements</typeparam>
        /// <param name="array">Initialised array</param>
        public static TElement[][] MakeSquare<TElement>(this TElement[][] array)
        {
            return MakeRectangle(array, array.Length);
        }
    }
}