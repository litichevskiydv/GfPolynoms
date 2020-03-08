namespace AppliedAlgebra.GfPolynoms
{
    using System;
    using GaloisFields;

    /// <summary>
    /// Wrapper for operating with field elements in object manner
    /// </summary>
    public class FieldElement
    {
        /// <summary>
        /// Method for checking the equality of the current field element to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another field element</param>
        /// <returns>Checking result</returns>
        private bool Equals(FieldElement other)
        {
            return Equals(Field, other.Field) && Representation == other.Representation;
        }

        /// <summary>
        /// Method for checking that field element has field equal to current element's field
        /// </summary>
        /// <param name="b">Validated field element</param>
        private void ValidateArgument(FieldElement b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException($"Field {Field}({Field.GetType()}) is not compatible to field {b.Field}({b.Field.GetType()})");
        }

        /// <summary>
        /// Field which current element belongs
        /// </summary>
        public GaloisField Field { get; }
        /// <summary>
        /// Current element number representation
        /// </summary>
        public int Representation { get; private set; }

        /// <summary>
        /// Constructor for creation element belongs to <paramref name="field"/> with number representation <paramref name="representation"/>
        /// </summary>
        /// <param name="field">Field which element belongs</param>
        /// <param name="representation">Element number representation</param>
        public FieldElement(GaloisField field, int representation)
        {
            if (field == null)
                throw new ArgumentException("field");
            if(field.IsFieldElement(representation) == false)
                throw new ArgumentException($"{representation} is not an element of {field}");

            Field = field;
            Representation = representation;
        }

        /// <summary>
        /// Constructor for creation copy of element <paramref name="element"/>
        /// </summary>
        /// <param name="element">Copied field element</param>
        public FieldElement(FieldElement element)
        {
            Field = element.Field;
            Representation = element.Representation;
        }

        /// <summary>
        /// Method for checking the equality of the current field element to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>Checking result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FieldElement) obj);
        }

        /// <summary>
        /// Method for calculation object hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Field.GetHashCode()*397) ^ Representation;
            }
        }

        /// <summary>
        /// Method for obtaining a string representation of the current field element
        /// </summary>
        /// <returns>String representation of the current field element</returns>
        public override string ToString()
        {
            return $"{Representation}";
        }

        /// <summary>
        /// Method for adding field element <paramref name="b"/> to current field element
        /// </summary>
        /// <param name="b">Second term</param>
        /// <returns>Sum</returns>
        public FieldElement Add(FieldElement b)
        {
            ValidateArgument(b);

            Representation = Field.Add(Representation, b.Representation);
            return this;
        }

        /// <summary>
        /// Method for subtracting field element <paramref name="b"/> from current field element
        /// </summary>
        /// <param name="b">Subtrahend</param>
        /// <returns>Difference</returns>
        public FieldElement Subtract(FieldElement b)
        {
            ValidateArgument(b);

            Representation = Field.Subtract(Representation, b.Representation);
            return this;
        }

        /// <summary>
        /// Method for multiplying field element <paramref name="b"/> to current field element
        /// </summary>
        /// <param name="b">Second factor</param>
        /// <returns>Product</returns>
        public FieldElement Multiply(FieldElement b)
        {
            ValidateArgument(b);

            Representation = Field.Multiply(Representation, b.Representation);
            return this;
        }

        /// <summary>
        /// Method for dividing current field element by field element <paramref name="b"/>
        /// </summary>
        /// <param name="b">Divider</param>
        /// <returns>Quotient</returns>
        public FieldElement Divide(FieldElement b)
        {
            ValidateArgument(b);
            if (b.Representation == 0)
                throw new ArgumentException("Cannot divide by zero");

            Representation = Field.Divide(Representation, b.Representation);
            return this;
        }

        /// <summary>
        /// Exponentiation of current field element to the degree <paramref name="degree"/>
        /// </summary>
        /// <param name="degree">Power for exponentiation</param>
        public FieldElement Pow(int degree)
        {
            Representation = Field.Pow(Representation, degree);
            return this;
        }

        /// <summary>
        /// Inverts current field element for addition
        /// </summary>
        public FieldElement InverseForAddition()
        {
            Representation = Field.InverseForAddition(Representation);
            return this;
        }

        /// <summary>
        /// Inverts current field element for multiplication
        /// </summary>
        public FieldElement InverseForMultiplication()
        {
            Representation = Field.InverseForMultiplication(Representation);
            return this;
        }

        /// <summary>
        /// Method for adding field element <paramref name="a"/> to field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">First term</param>
        /// <param name="b">Second term</param>
        /// <returns>Sum</returns>
        public static FieldElement Add(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Add(b);
        }

        /// <summary>
        /// Method for subtracting field element <paramref name="b"/> from field element <paramref name="a"/>
        /// </summary>
        /// <param name="a">Minuend</param>
        /// <param name="b">Subtrahend</param>
        /// <returns>Difference</returns>
        public static FieldElement Subtract(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Subtract(b);
        }

        /// <summary>
        /// Method for multiplying field element <paramref name="a"/> to field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        /// <returns>Product</returns>
        public static FieldElement Multiply(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Multiply(b);
        }

        /// <summary>
        /// Method for dividing field element <paramref name="a"/> by field element <paramref name="b"/>
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divider</param>
        /// <returns>Quotient</returns>
        public static FieldElement Divide(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Divide(b);
        }

        /// <summary>
        /// Exponentiation of field element <paramref name="a"/> to the degree <paramref name="degree"/>
        /// </summary>
        /// <param name="a">Element for exponentiation</param>
        /// <param name="degree">Power for exponentiation</param>
        public static FieldElement Pow(FieldElement a, int degree)
        {
            var c = new FieldElement(a);
            return c.Pow(degree);
        }

        /// <summary>
        /// Inverts field element <paramref name="a"/> for addition
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inversed element</returns>
        public static FieldElement InverseForAddition(FieldElement a)
        {
            var b = new FieldElement(a);
            return b.InverseForAddition();
        }

        /// <summary>
        /// Inverts field element <paramref name="a"/> for multiplication
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inversed element</returns>
        public static FieldElement InverseForMultiplication(FieldElement a)
        {
            var b = new FieldElement(a);
            return b.InverseForMultiplication();
        }

        public static FieldElement operator +(FieldElement a, FieldElement b)
        {
            return Add(a, b);
        }

        public static FieldElement operator -(FieldElement a)
        {
            return InverseForAddition(a);
        }

        public static FieldElement operator -(FieldElement a, FieldElement b)
        {
            return Subtract(a, b);
        }

        public static FieldElement operator *(FieldElement a, FieldElement b)
        {
            return Multiply(a, b);
        }

        public static FieldElement operator /(FieldElement a, FieldElement b)
        {
            return Divide(a, b);
        }
    }
}