namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Collections.Concurrent;

    public class PrimeOrderField : IGaluaField
    {
        private class GcdSearchResult
        {
            public int Gcd { get; }
            public int X { get; set; }
            public int Y { get; set; }

            public GcdSearchResult(int gcd, int x, int y)
            {
                Gcd = gcd;
                X = x;
                Y = y;
            }
        }

        private readonly ConcurrentDictionary<int, int> _inverseElementsByMultiply;

        private void ValidateArguments(int a, int b)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (IsFieldElement(b) == false)
                throw new ArgumentException($"Element {b} is not field member");
        }

        /// <summary>
        /// Find x and y such that a*x + b*y = gcd(a,b) 
        /// </summary>
        private static GcdSearchResult ExtendedGrandCommonDivisor(int a, int b)
        {
            if (a == 0)
                return new GcdSearchResult(b, 0, 1);

            var result = ExtendedGrandCommonDivisor(b%a, a);
            var xOld = result.X;
            result.X = result.Y - (b/a)*xOld;
            result.Y = xOld;

            return result;
        }

        private int CalculateInverseElementByMultiply(int element)
        {
            var gcdSearchResult = ExtendedGrandCommonDivisor(element, Order);
            if (gcdSearchResult.Gcd != 1)
                throw new InvalidOperationException($"Field order {Order} is not prime");
            return (gcdSearchResult.X + Order)%Order;
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
            _inverseElementsByMultiply = new ConcurrentDictionary<int, int>();
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

            var inverseElement = _inverseElementsByMultiply.GetOrAdd(a, CalculateInverseElementByMultiply);
            _inverseElementsByMultiply.TryAdd(inverseElement, a);

            return inverseElement;
        }
    }
}