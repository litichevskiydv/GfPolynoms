namespace AppliedAlgebra.GfPolynoms.GaloisFields
{
    using System;
    using System.Collections.Generic;

    public abstract class GaloisField
    {
        /// <summary>
        /// Field's elements representation, which can be accessed by powers of field's generating element
        /// </summary>
        protected int[] ElementsByPowers { get; private set; }
        /// <summary>
        /// Powers of field's generating element, which can be accessed by field's elements representation
        /// </summary>
        protected int[] PowersByElements { get; private set; }


        /// <summary>
        /// Field order
        /// </summary>
        public int Order { get; private set; }
        /// <summary>
        /// Field characteristic
        /// </summary>
        public int Characteristic { get; private set; }

        /// <summary>
        /// Method for analyzing field order factors
        /// </summary>
        /// <param name="order">Field order </param>
        /// <returns>First field order factor and its degree</returns>
        protected static Dictionary<int, int> AnalyzeOrder(int order)
        {
            var fractions = new Dictionary<int, int>();

            for (var i = 2; i*i <= order && order > 1; i++)
            {
                if(order % i != 0)
                    continue;

                fractions[i] = 0;
                if (fractions.Count > 1)
                    return fractions;

                while (order % i == 0)
                {
                    fractions[i]++;
                    order /= i;
                }
            }

            if (order != 1)
                fractions[order] = 1;
            return fractions;
        }

        /// <summary>
        /// Method for initializing field's object internal structures
        /// </summary>
        /// <param name="order">Field order</param>
        /// <param name="characteristic">Field characteristic</param>
        protected void Initialize(int order, int characteristic)
        {
            Order = order;
            Characteristic = characteristic;

            PowersByElements = new int[order];
            PowersByElements[0] = -1;

            ElementsByPowers = new int[order - 1];
        }

        /// <summary>
        /// Method for checking whether the operands are field elements
        /// </summary>
        protected void ValidateArguments(int a, int b)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (IsFieldElement(b) == false)
                throw new ArgumentException($"Element {b} is not field member");
        }

        /// <summary>
        /// The method for checking whether the operand is an element of the field
        /// </summary>
        public bool IsFieldElement(int a)
        {
            return a >= 0 && a < Order;
        }

        /// <summary>
        /// Method for adding field element <paramref name="a"/> to field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">First term</param>
        /// <param name="b">Second term</param>
        /// <returns>Sum</returns>
        public abstract int Add(int a, int b);

        /// <summary>
        /// Method for subtracting field element <paramref name="b"/> from field element <paramref name="a"/>
        /// </summary>
        /// <param name="a">Minuend</param>
        /// <param name="b">Subtrahend</param>
        /// <returns>Difference</returns>
        public abstract int Subtract(int a, int b);

        /// <summary>
        /// Method for multiplying field element <paramref name="a"/> to field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        /// <returns>Product</returns>
        public int Multiply(int a, int b)
        {
            ValidateArguments(a, b);

            if (a == 0 || b == 0)
                return 0;
            return ElementsByPowers[(PowersByElements[a] + PowersByElements[b]) % (Order - 1)];
        }

        /// <summary>
        /// Method for dividing field element <paramref name="a"/> by field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divider</param>
        /// <returns>Quotient</returns>
        public int Divide(int a, int b)
        {
            ValidateArguments(a, b);
            if (b == 0)
                throw new ArgumentException(nameof(b));

            return a == 0
                ? 0
                : ElementsByPowers[(PowersByElements[a] - PowersByElements[b] + Order - 1) % (Order - 1)];
        }

        /// <summary>
        /// Inverts field element
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inversed element</returns>
        public abstract int InverseForAddition(int a);

        /// <summary>
        /// Inverts field element
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inversed element</returns>
        public int InverseForMultiplication(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (a == 0)
                throw new ArgumentException("Can't inverse zero");

            return ElementsByPowers[(Order - 1 - PowersByElements[a]) % (Order - 1)];
        }

        /// <summary>
        /// Exponentiation of the generation element to the specified degree
        /// </summary>
        /// <param name="degree">Power for exponentiation</param>
        /// <returns>Exponentiation result</returns>
        public int GetGeneratingElementPower(int degree)
        {
            return degree >= 0
                ? ElementsByPowers[degree % (Order - 1)]
                : InverseForMultiplication(ElementsByPowers[(-degree) % (Order - 1)]);
        }

        /// <summary>
        /// Exponentiation of field element <paramref name="element"/> to the degree <paramref name="degree"/>
        /// </summary>
        /// <param name="element">Element for exponentiation</param>
        /// <param name="degree">Power for exponentiation</param>
        public int Pow(int element, int degree)
        {
            if (IsFieldElement(element) == false)
                throw new ArgumentException($"Element {element} is not field member");

            if (degree == 0)
                return 1;
            return element == 0 ? 0 : GetGeneratingElementPower(PowersByElements[element]*degree);
        }
    }
}