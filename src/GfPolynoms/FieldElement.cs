namespace GfPolynoms
{
    using System;
    using GaluaFields;

    public class FieldElement
    {
        private bool Equals(FieldElement other)
        {
            return Equals(Field, other.Field) && Representation == other.Representation;
        }

        public GaluaField Field { get; }
        public int Representation { get; private set; }

        public FieldElement(GaluaField field, int representation)
        {
            if (field == null)
                throw new ArgumentException("field");
            if(field.IsFieldElement(representation) == false)
                throw new ArgumentException($"{representation} is not an element of {field}");

            Field = field;
            Representation = representation;
        }

        public FieldElement(FieldElement element)
        {
            Field = element.Field;
            Representation = element.Representation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FieldElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Field.GetHashCode()*397) ^ Representation;
            }
        }

        public override string ToString()
        {
            return $"{Representation}";
        }

        public FieldElement Add(FieldElement b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

            Representation = Field.Add(Representation, b.Representation);
            return this;
        }

        public FieldElement Subtract(FieldElement b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

            Representation = Field.Subtract(Representation, b.Representation);
            return this;
        }

        public FieldElement Multiply(FieldElement b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

            Representation = Field.Multiply(Representation, b.Representation);
            return this;
        }

        public FieldElement Divide(FieldElement b)
        {
            if (Field.Equals(b.Field) == false || b.Representation == 0)
                throw new ArgumentException("b");

            Representation = Field.Divide(Representation, b.Representation);
            return this;
        }

        public FieldElement Pow(int degree)
        {
            Representation = Field.Pow(Representation, degree);
            return this;
        }

        public static FieldElement Add(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Add(b);
        }

        public static FieldElement Subtract(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Subtract(b);
        }

        public static FieldElement Multiply(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Multiply(b);
        }

        public static FieldElement Divide(FieldElement a, FieldElement b)
        {
            var c = new FieldElement(a);
            return c.Divide(b);
        }

        public static FieldElement Pow(FieldElement a, int degree)
        {
            var c = new FieldElement(a);
            return c.Pow(degree);
        }

        public static FieldElement operator +(FieldElement a, FieldElement b)
        {
            return Add(a, b);
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