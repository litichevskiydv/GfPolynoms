namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Linq;

    public class PrimeOrderField : GaluaField
    {
        private void BuildMultiplicativeGroup()
        {
            for (var i = 1; i < Order; i++)
            {
                for (int newElement = 1, power = 0;
                    PowersByElements.ContainsKey(newElement) == false;
                    newElement = (newElement*i)%Order, power++)
                    PowersByElements[newElement] = power;

                if (PowersByElements.Count == Order - 1)
                {
                    ElementsByPowers = PowersByElements.ToDictionary(x => x.Value, x => x.Key);
                    break;
                }
                PowersByElements.Clear();
            }
        }

        private bool Equals(PrimeOrderField other)
        {
            return Order == other.Order;
        }

        /// <summary>
        ///     Создаем поле
        /// </summary>
        /// <param name="order">Порядок поля, простое число</param>
        public PrimeOrderField(int order) : base(order, order)
        {
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

        public override int Add(int a, int b)
        {
            ValidateArguments(a, b);

            return (a + b)%Order;
        }

        public override int Subtract(int a, int b)
        {
            ValidateArguments(a, b);

            return (a - b + Order)%Order;
        }

        public override int InverseForAddition(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");

            return (Order - a)%Order;
        }
    }
}