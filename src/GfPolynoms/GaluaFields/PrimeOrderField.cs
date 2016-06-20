namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PrimeOrderField : IGaluaField
    {
        private Dictionary<int, int> _elementsByPowers;
        private readonly Dictionary<int, int> _powersByElements;

        public int Order { get; }
        public int Characteristic => Order;

        private void BuildMultiplicativeGroup()
        {
            for (var i = 1; i < Order; i++)
            {
                for (int newElement = 1, power = 0;
                    !_powersByElements.ContainsKey(newElement);
                    newElement = (newElement*i)%Order, power++)
                    _powersByElements[newElement] = power;

                if (_powersByElements.Count == Order - 1)
                {
                    _elementsByPowers = _powersByElements.ToDictionary(x => x.Value, x => x.Key);
                    break;
                }
                _powersByElements.Clear();
            }
        }

        private void ValidateArguments(int a, int b)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (IsFieldElement(b) == false)
                throw new ArgumentException($"Element {b} is not field member");
        }

        private bool Equals(PrimeOrderField other)
        {
            return Order == other.Order;
        }

        /// <summary>
        ///     Создаем поле
        /// </summary>
        /// <param name="order">Порядок поля, простое число</param>
        public PrimeOrderField(int order)
        {
            Order = order;

            _elementsByPowers = new Dictionary<int, int>();
            _powersByElements = new Dictionary<int, int>();
            BuildMultiplicativeGroup();
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
                throw new ArgumentException(nameof(b));

            return a == 0 ? 0 : Multiply(a, InverseForMultiplication(b));
        }

        public int InverseForAddition(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");

            return (Order - a)%Order;
        }

        public int InverseForMultiplication(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if(a == 0)
                throw new ArgumentException("Can't inverse zero");

            return _elementsByPowers[(Order - 1 - _powersByElements[a])%(Order - 1)];
        }

        public int GetGeneratingElementPower(int degree)
        {
            return degree >= 0
                ? _elementsByPowers[degree%(Order - 1)]
                : InverseForMultiplication(_elementsByPowers[(-degree) % (Order - 1)]);
        }

        public int Pow(int element, int degree)
        {
            if (IsFieldElement(element) == false)
                throw new ArgumentException($"Element {element} is not field member");

            return element == 0 ? 0 : GetGeneratingElementPower(_powersByElements[element]*degree);
        }
    }
}