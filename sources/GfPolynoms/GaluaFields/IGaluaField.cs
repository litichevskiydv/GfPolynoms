namespace GfPolynoms.GaluaFields
{
    public interface IGaluaField
    {
        /// <summary>
        /// Порядок поля, простое число в некоторой степени
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Характеристика поля, степень простого числа в порядке поля
        /// </summary>
        int Characteristic { get; }

        /// <summary>
        /// Проверка, входит ли переданное значение в поле
        /// </summary>
        bool IsFieldElement(int a);

        /// <summary>
        /// Сложение двух элементов поля
        /// </summary>
        /// <param name="a">Первое слагаемое</param>
        /// <param name="b">Второе слагаемое</param>
        /// <returns>Сумма</returns>
        int Add(int a, int b);

        /// <summary>
        /// Вычетание двух элементов поля
        /// </summary>
        /// <param name="a">Уменьшаемое</param>
        /// <param name="b">Вычетаемое</param>
        /// <returns>Разность</returns>
        int Subtract(int a, int b);

        /// <summary>
        /// Умножение двух элементов поля
        /// </summary>
        /// <param name="a">Первый множитель</param>
        /// <param name="b">Второй множитель</param>
        /// <returns>Произведение</returns>
        int Multiply(int a, int b);
    }
}