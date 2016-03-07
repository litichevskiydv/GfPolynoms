namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class PrimePowerOrderField : IGaluaField
    {
        private class GcdSearchResult
        {
            public Polynomial Gcd { get; }
            public Polynomial X { get; set; }
            public Polynomial Y { get; set; }

            public GcdSearchResult(Polynomial gcd, Polynomial x, Polynomial y)
            {
                Gcd = gcd;
                X = x;
                Y = y;
            }
        }

        private readonly Dictionary<int, Polynomial> _polynomialByRepresentation;
        private readonly Dictionary<Polynomial, int> _representationByPolynomial;
        private readonly ConcurrentDictionary<int, int> _inverseElementsByMultiply;

        private static int CalculateElementRepresentation(int characteristic, IEnumerable<int> coefficients)
        {
            return coefficients.Aggregate(0, (current, coefficient) => current*characteristic + coefficient);
        }

        private void GenerateFieldElements(int characteristic, int[] coefficients, int position)
        {
            if (position == -1)
            {
                var representation = CalculateElementRepresentation(characteristic, coefficients.Reverse());
                var polynomial = new Polynomial(IrreduciblePolynomial.Field, coefficients);
                _polynomialByRepresentation[representation] = polynomial;
                _representationByPolynomial[polynomial] = representation;
                return;
            }

            for (var i = 0; i < characteristic; i++)
            {
                coefficients[position] = i;
                GenerateFieldElements(characteristic, coefficients, position - 1);
            }
        }

        /// <summary>
        /// Find x and y such that a*x + b*y = gcd(a,b) 
        /// </summary>
        private static GcdSearchResult ExtendedGrandCommonDivisor(Polynomial a, Polynomial b)
        {
            if (a.IsZero)
                return new GcdSearchResult(b, new Polynomial(a.Field), new Polynomial(a.Field, 1));

            var divisionResult = b.DivideEx(a);
            var result = ExtendedGrandCommonDivisor(divisionResult.Item2, a);
            var xOld = result.X;
            result.X = result.Y - divisionResult.Item1 * xOld;
            result.Y = xOld;

            return result;
        }

        private int CalculateInverseElementByMultiply(int element)
        {
            var gcdSearchResult = ExtendedGrandCommonDivisor(_polynomialByRepresentation[element], IrreduciblePolynomial);
            if (gcdSearchResult.Gcd.Degree != 0)
                throw new InvalidOperationException($"Polynomial {IrreduciblePolynomial} isn't irreducible");
            return _representationByPolynomial[gcdSearchResult.X/gcdSearchResult.Gcd];
        }

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

        public PrimePowerOrderField(int order, int characteristic, int[] irreduciblePolynomial)
        {
            if (order < 2)
                throw new ArgumentException("order");
            if (characteristic < 2)
                throw new ArgumentException("characteristic");
            if (irreduciblePolynomial == null)
                throw new ArgumentException("irreduciblePolynomial");

            Order = order;
            Characteristic = characteristic;
            IrreduciblePolynomial = new Polynomial(new PrimeOrderField(characteristic), irreduciblePolynomial);
            _inverseElementsByMultiply = new ConcurrentDictionary<int, int>();

            _representationByPolynomial = new Dictionary<Polynomial, int>();
            _polynomialByRepresentation = new Dictionary<int, Polynomial>();
            GenerateFieldElements(characteristic, new int[IrreduciblePolynomial.Degree], IrreduciblePolynomial.Degree - 1);
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

            return _representationByPolynomial[_polynomialByRepresentation[a] + _polynomialByRepresentation[b]];
        }

        public int Subtract(int a, int b)
        {
            ValidateArguments(a, b);

            return _representationByPolynomial[_polynomialByRepresentation[a] - _polynomialByRepresentation[b]];
        }

        public int Multiply(int a, int b)
        {
            ValidateArguments(a, b);

            if (a == 0 || b == 0)
                return 0;

            return _representationByPolynomial[(_polynomialByRepresentation[a]*_polynomialByRepresentation[b])%IrreduciblePolynomial];
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

            return _representationByPolynomial[_polynomialByRepresentation[0] - _polynomialByRepresentation[a]];
        }

        public int InverseForMultiplication(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");
            if (a == 0)
                throw new ArgumentException("Can't inverse zero");

            var inverseElement = _inverseElementsByMultiply.GetOrAdd(a, CalculateInverseElementByMultiply);
            _inverseElementsByMultiply.TryAdd(inverseElement, a);

            return inverseElement;
        }
    }
}