namespace GfAlgorithms.Extensions
{
    public static class ArraysExtensions
    {
        public static TElement[][] MakeRectangle<TElement>(this TElement[][] array, int secondDimensionLength)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = new TElement[secondDimensionLength];
            return array;
        }

        public static TElement[][] MakeSquare<TElement>(this TElement[][] array)
        {
            return MakeRectangle(array, array.Length);
        }
    }
}