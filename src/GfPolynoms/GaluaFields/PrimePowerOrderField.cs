namespace GfPolynoms.GaluaFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PrimePowerOrderField : IGaluaField
    {
        private Dictionary<int, int> _elementsByPowers;
        private readonly Dictionary<int, int> _powersByElements;
        private readonly Dictionary<int, Polynomial> _polynomialByRepresentation;
        private readonly Dictionary<Polynomial, int> _representationByPolynomial;

        public int Order { get; }
        public int Characteristic { get; }
        public Polynomial IrreduciblePolynomial { get; }

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

        private void BuildMultiplicativeGroup()
        {
            for (var i = 2; i < Order; i++)
            {
                for (int newElement = 1, power = 0;
                    !_powersByElements.ContainsKey(newElement);
                    newElement = _representationByPolynomial[(_polynomialByRepresentation[newElement]*_polynomialByRepresentation[i])%IrreduciblePolynomial], power++)
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

        private bool Equals(PrimePowerOrderField other)
        {
            return IrreduciblePolynomial.Equals(other.IrreduciblePolynomial) && Order == other.Order;
        }

        public PrimePowerOrderField(int order, int characteristic, int[] irreduciblePolynomial)
        {
            if (order < 2)
                throw new ArgumentException(nameof(order));
            if (characteristic < 2)
                throw new ArgumentException(nameof(characteristic));
            if (irreduciblePolynomial == null)
                throw new ArgumentNullException(nameof(irreduciblePolynomial));

            Order = order;
            Characteristic = characteristic;
            IrreduciblePolynomial = new Polynomial(new PrimeOrderField(characteristic), irreduciblePolynomial);

            _representationByPolynomial = new Dictionary<Polynomial, int>();
            _polynomialByRepresentation = new Dictionary<int, Polynomial>();
            GenerateFieldElements(characteristic, new int[IrreduciblePolynomial.Degree], IrreduciblePolynomial.Degree - 1);

            _elementsByPowers = new Dictionary<int, int>();
            _powersByElements = new Dictionary<int, int>();
            BuildMultiplicativeGroup();
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

            return _elementsByPowers[(_powersByElements[a] + _powersByElements[b])%(Order - 1)];
        }

        public int Divide(int a, int b)
        {
            ValidateArguments(a, b);
            if (b == 0)
                throw new ArgumentException(nameof(b));

            return a == 0
                ? 0
                : _elementsByPowers[(_powersByElements[a] - _powersByElements[b] + Order - 1)%(Order - 1)];
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

            return _elementsByPowers[(Order - 1 - _powersByElements[a])%(Order - 1)];
        }

        public int GetGeneratingElementPower(int power)
        {
            return power >= 0
                ? _elementsByPowers[power % (Order - 1)]
                : InverseForMultiplication(_elementsByPowers[(-power) % (Order - 1)]);
        }
    }
}