namespace GfPolynoms.GaluaFields
{
    public interface IGaluaField
    {
        /// <summary>
        ///     Порядок поля, простое число в некоторой степени
        /// </summary>
        int Order { get; }

        /// <summary>
        ///     Характеристика поля, степень простого числа в порядке поля
        /// </summary>
        int Characteristic { get; }

        /// <summary>
        ///     Проверка, входит ли переданное значение в поле
        /// </summary>
        bool IsFieldElement(int a);

        /// <summary>
        ///     Сложение двух элементов поля
        /// </summary>
        /// <param name="a">Первое слагаемое</param>
        /// <param name="b">Второе слагаемое</param>
        /// <returns>Сумма</returns>
        int Add(int a, int b);

        /// <summary>
        ///     Вычетание двух элементов поля
        /// </summary>
        /// <param name="a">Уменьшаемое</param>
        /// <param name="b">Вычетаемое</param>
        /// <returns>Разность</returns>
        int Subtract(int a, int b);

        /// <summary>
        ///     Умножение двух элементов поля
        /// </summary>
        /// <param name="a">Первый множитель</param>
        /// <param name="b">Второй множитель</param>
        /// <returns>Произведение</returns>
        int Multiply(int a, int b);

        /// <summary>
        ///     Деление двух элементов поля
        /// </summary>
        /// <param name="a">Делимое</param>
        /// <param name="b">Делитель</param>
        /// <returns>Частное</returns>
        int Divide(int a, int b);

        /// <summary>
        /// Inverts field element
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inverse osite element</returns>
        int InverseForAddition(int a);

        /// <summary>
        /// Inverts field element
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inverse osite element</returns>
        int InverseForMultiplication(int a);

        /// <summary>
        /// Exponentiation of the generation element to the specified degree
        /// </summary>
        /// <param name="degree">Power for exponentiation</param>
        /// <returns>Exponentiation result</returns>
        int GetGeneratingElementPower(int degree);

        /// <summary>
        /// Exponentiation of given element to the specified degree
        /// </summary>
        /// <param name="element">Element for exponentiation</param>
        /// <param name="degree">Power for exponentiation</param>
        int Pow(int element, int degree);
    }
}