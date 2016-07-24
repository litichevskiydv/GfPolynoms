namespace GfPolynoms.GaloisFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PrimePowerOrderField : GaloisField
    {
        private readonly Polynomial[] _polynomialByRepresentation;
        private readonly Dictionary<Polynomial, int> _representationByPolynomial;

        private readonly int[][] _additionResults;
        private readonly int[][] _subtractionResults;

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
                var generationResult = new HashSet<int>();
                for (int newElement = 1, power = 0; generationResult.Add(newElement); power++)
                {
                    PowersByElements[newElement] = power;
                    ElementsByPowers[power] = newElement;

                    newElement =
                        _representationByPolynomial[
                            (_polynomialByRepresentation[newElement]*_polynomialByRepresentation[i])%IrreduciblePolynomial];
                }

                if (generationResult.Count == Order - 1)
                    break;
            }
        }
        private void PrecalculateAdditionResults()
        {
            for (var i = 0; i < Order; i++)
            {
                _additionResults[i] = new int[Order];
                for (var j = 0; j < Order; j++)
                    _additionResults[i][j] = _representationByPolynomial[_polynomialByRepresentation[i] + _polynomialByRepresentation[j]];
            }
        }

        private void PrecalculateSubtractionResults()
        {
            for (var i = 0; i < Order; i++)
            {
                _subtractionResults[i] = new int[Order];
                for (var j = 0; j < Order; j++)
                    _subtractionResults[i][j] = _representationByPolynomial[_polynomialByRepresentation[i] - _polynomialByRepresentation[j]];
            }
        }

        public PrimePowerOrderField(int order, int characteristic, int[] irreduciblePolynomial) : base(order, characteristic)
        {
            if (irreduciblePolynomial == null)
                throw new ArgumentNullException(nameof(irreduciblePolynomial));

            IrreduciblePolynomial = new Polynomial(new PrimeOrderField(characteristic), irreduciblePolynomial);

            _representationByPolynomial = new Dictionary<Polynomial, int>();
            _polynomialByRepresentation = new Polynomial[order];
            GenerateFieldElements(characteristic, new int[IrreduciblePolynomial.Degree], IrreduciblePolynomial.Degree - 1);

            BuildMultiplicativeGroup();

            _additionResults = new int[order][];
            PrecalculateAdditionResults();

            _subtractionResults = new int[order][];
            PrecalculateSubtractionResults();
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

        public override int Add(int a, int b)
        {
            ValidateArguments(a, b);

            return _additionResults[a][b];
        }

        public override int Subtract(int a, int b)
        {
            ValidateArguments(a, b);

            return _subtractionResults[a][b];
        }

        public override int InverseForAddition(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");

            return _subtractionResults[0][a];
        }
    }
}