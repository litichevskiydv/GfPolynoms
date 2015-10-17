using System;

namespace GfPolynoms.GaluaFields
{
    public class PrimeOrderField : IGaluaField
    {
        private void ValidateArguments(int a, int b)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (IsFieldElement(b) == false)
                throw new ArgumentException($"Element {b} is not field member");
        }

        /// <summary>
        /// Создаем поле
        /// </summary>
        /// <param name="order">Порядок поля, простое число</param>
        public PrimeOrderField(int order)
        {
            Order = order;
        }

        public int Order { get; }
        public int Characteristic => 1;

        public bool IsFieldElement(int a)
        {
            return a >= 0 && a < Order;
        }

        public int Add(int a, int b)
        {
            ValidateArguments(a, b);

            return (a + b)%Order;
        }

        public int Subtract(int a, int b)
        {
            ValidateArguments(a, b);

            return (a - b + Order)%Order;
        }

        public int Multiply(int a, int b)
        {
            ValidateArguments(a, b);

            return (a*b)%Order;
        }
    }
}