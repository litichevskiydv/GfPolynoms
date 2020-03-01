namespace AppliedAlgebra.GfPolynoms.GaloisFields
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field with prime number order
    /// </summary>
    internal class PrimeOrderField : GaloisField
    {
        /// <summary>
        /// Method for calculating elements of field multiplicative group
        /// </summary>
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

        /// <summary>
        /// Method for checking the equality of the current field to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another field</param>
        /// <returns>Checking result</returns>
        private bool Equals(PrimeOrderField other)
        {
            return Order == other.Order;
        }

        /// <summary>
        /// Constructor for creation field with order <param name="order"></param>
        /// </summary>
        /// <param name="order">Field order, should be a prime number</param>
        public PrimeOrderField(int order)
        {
            Initialize(order, order);
            BuildMultiplicativeGroup();
        }

        /// <summary>
        /// Method for checking the equality of the current field to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>Checking result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PrimeOrderField) obj);
        }

        /// <summary>
        /// Method for calculation object hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            return Order;
        }

        /// <summary>
        /// Method for adding field element <paramref name="a"/> to field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">First term</param>
        /// <param name="b">Second term</param>
        /// <returns>Sum</returns>
        public override int Add(int a, int b)
        {
            ValidateArguments(a, b);

            return (a + b)%Order;
        }

        /// <summary>
        /// Method for subtracting field element <paramref name="b"/> from field element <paramref name="a"/>
        /// </summary>
        /// <param name="a">Minuend</param>
        /// <param name="b">Subtrahend</param>
        /// <returns>Difference</returns>
        public override int Subtract(int a, int b)
        {
            ValidateArguments(a, b);

            return (a - b + Order)%Order;
        }

        /// <summary>
        /// Inverts field element
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inverse osite element</returns>
        public override int InverseForAddition(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");

            return (Order - a)%Order;
        }

        /// <summary>
        /// Method for obtaining a string representation of the current field
        /// </summary>
        /// <returns>String representation of the current field</returns>
        public override string ToString()
        {
            return $"GF{Order}";
        }
    }
}