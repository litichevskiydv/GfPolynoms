namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Collections.Generic;

    public class PrimeOrderField : IGaluaField
    {
        private readonly Dictionary<int, int> _inverseElementsByMultiply;

        private void ValidateArguments(int a, int b)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (IsFieldElement(b) == false)
                throw new ArgumentException($"Element {b} is not field member");
        }

        private int CalculateInverseElementByMultiply(int element)
        {
            for (var i = 1; i < Order; i++)
                if (Multiply(element, i) == 1)
                    return i;
            throw new InvalidOperationException($"Field order {Order} is not prime");
        }

        private bool Equals(PrimeOrderField other)
        {
            return Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PrimeOrderField) obj);
        }

        public override int GetHashCode()
        {
            return Order;
        }

        /// <summary>
        ///     Создаем поле
        /// </summary>
        /// <param name="order">Порядок поля, простое число</param>
        public PrimeOrderField(int order)
        {
            Order = order;
            _inverseElementsByMultiply = new Dictionary<int, int>();
        }

        public int Order { get; }
        public int Characteristic => Order;

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

        public int Divide(int a, int b)
        {
            ValidateArguments(a, b);
            if (b == 0)
                throw new ArgumentException("b");

            if (a == 0)
                return 0;

            int inverseElement;
            lock (this)
            {
                if (_inverseElementsByMultiply.TryGetValue(b, out inverseElement) == false)
                {
                    inverseElement = CalculateInverseElementByMultiply(b);
                    _inverseElementsByMultiply[b] = inverseElement;
                    _inverseElementsByMultiply[inverseElement] = b;
                }
            }
            return Multiply(a, inverseElement);
        }
    }
}