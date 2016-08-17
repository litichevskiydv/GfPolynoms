namespace GfPolynoms.GaloisFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PrimeOrderField : GaloisField
    {
        private void BuildMultiplicativeGroup()
        {
            for (var i = 1; i < Order; i++)
            {
                var generationResult = new HashSet<int>();
                for (int newElement = 1, power = 0; generationResult.Add(newElement); power++)
                {
                    PowersByElements[newElement] = power;
                    ElementsByPowers[power] = newElement;

                    newElement = (newElement*i)%Order;
                }

                if (generationResult.Count == Order - 1)
                    break;
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
        public PrimeOrderField(int order)
        {
            var analysisResult = AnalyzeOrder(order);
            if(analysisResult.Count != 1 || analysisResult.First().Value != 1)
                throw new ArgumentException("Field order isn't a prime number");

            Initialize(order, order);
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