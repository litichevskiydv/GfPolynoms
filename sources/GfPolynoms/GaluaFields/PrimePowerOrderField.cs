namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Collections.Generic;

    public class PrimePowerOrderField : IGaluaField
    {
        private readonly Dictionary<int, Polynomial> _polynomialByRepresentation;
        private readonly Dictionary<Polynomial, int> _representationByPolynomial;

        private void ValidateArguments(int a, int b)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (IsFieldElement(b) == false)
                throw new ArgumentException($"Element {b} is not field member");
        }

        private bool Equals(PrimePowerOrderField other)
        {
            return IrreduciblePolynomial.Equals(other.IrreduciblePolynomial) && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PrimePowerOrderField) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IrreduciblePolynomial.GetHashCode()*397) ^ Order;
            }
        }

        public Polynomial this[int index]
        {
            get
            {
                if (IsFieldElement(index) == false)
                    throw new IndexOutOfRangeException();

                return _polynomialByRepresentation[index];
            }
        }

        public PrimePowerOrderField( int order, int characteristic, IEnumerable<int> irreduciblePolynomial)
        {
            if(order < 2)
                throw new ArgumentException("order");
            if (characteristic < 2)
                throw new ArgumentException("characteristic");
            if (irreduciblePolynomial == null)
                throw new ArgumentException("irreduciblePolynomial");

            Order = order;
            Characteristic = characteristic;
            IrreduciblePolynomial = new Polynomial(new PrimeOrderField(characteristic), irreduciblePolynomial);

            var zero = new Polynomial(IrreduciblePolynomial.Field);
            var one = new Polynomial(IrreduciblePolynomial.Field, new[] {1});
            _polynomialByRepresentation = new Dictionary<int, Polynomial> {{0, zero}};
            _representationByPolynomial = new Dictionary<Polynomial, int> {{zero, 0}};
            for (var i = 1; i < order; i++)
            {
                var polynomial = (one >> (i - 1))% IrreduciblePolynomial;
                _polynomialByRepresentation[i] = polynomial;
                _representationByPolynomial[polynomial] = i;
            }
        }

        public int Order { get; }
        public int Characteristic { get; }
        public Polynomial IrreduciblePolynomial { get; }

        public bool IsFieldElement(int a)
        {
            return a >= 0 && a < Order;
        }

        public int Add(int a, int b)
        {
            ValidateArguments(a, b);

            var c = (_polynomialByRepresentation[a] + _polynomialByRepresentation[b])%IrreduciblePolynomial;
            return _representationByPolynomial[c];
        }

        public int Subtract(int a, int b)
        {
            ValidateArguments(a, b);

            var c = (_polynomialByRepresentation[a] - _polynomialByRepresentation[b]) % IrreduciblePolynomial;
            return _representationByPolynomial[c];
        }

        public int Multiply(int a, int b)
        {
            ValidateArguments(a, b);

            if (a == 0 || b == 0)
                return 0;

            return ((a - 1)+(b - 1))%(Order - 1) + 1;
        }

        public int Divide(int a, int b)
        {
            ValidateArguments(a, b);
            if (b == 0)
                throw new ArgumentException("b");

            if (a == 0)
                return 0;

            return ((a - 1) - (b - 1) + (Order - 1))%(Order - 1) + 1;
        }
    }
}