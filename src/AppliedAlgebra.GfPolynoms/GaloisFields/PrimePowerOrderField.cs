namespace AppliedAlgebra.GfPolynoms.GaloisFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IrreduciblePolynomialsFinder;

    public class PrimePowerOrderField : GaloisField
    {
        private Polynomial[] _polynomialByRepresentation;
        private Dictionary<Polynomial, int> _representationByPolynomial;

        private int[][] _additionResults;
        private int[][] _subtractionResults;

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
                if(i == Order - 1)
                    throw new InvalidOperationException("Can't construct field multiplicative group");
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

        /// <summary>
        /// Метод для инициализации дополнительных элементов поля
        /// </summary>
        private void InitializeAdditionalStructures()
        {
            _representationByPolynomial = new Dictionary<Polynomial, int>();
            _polynomialByRepresentation = new Polynomial[Order];
            GenerateFieldElements(Characteristic, new int[IrreduciblePolynomial.Degree], IrreduciblePolynomial.Degree - 1);

            BuildMultiplicativeGroup();

            _additionResults = new int[Order][];
            PrecalculateAdditionResults();

            _subtractionResults = new int[Order][];
            PrecalculateSubtractionResults();
        }

        /// <summary>
        /// Конструктор для создания поля, в качестве параметра передается его порядок <paramref name="order"/>
        /// </summary>
        /// <param name="order">Порядок поля</param>
        public PrimePowerOrderField(int order) : this(order, new SimpleFinder())
        {
        }

        /// <summary>
        /// Конструктор для создания поля, в качестве параметра передается его порядок <paramref name="order"/> и генератор неприводимых многочленов <paramref name="irreduciblePolynomialsFinder"/>
        /// </summary>
        /// <param name="order">Порядок поля</param>
        /// <param name="irreduciblePolynomialsFinder">Генератор неприводимых многочленов</param>
        public PrimePowerOrderField(int order, IIrreduciblePolynomialsFinder irreduciblePolynomialsFinder)
        {
            var analysisResult = AnalyzeOrder(order);
            if (analysisResult.Count != 1)
                throw new ArgumentException("Field order isn't a prime number power");
            if (analysisResult.First().Value == 1)
                throw new ArgumentException("Field order is a prime number");

            if (irreduciblePolynomialsFinder == null)
                throw new ArgumentNullException(nameof(irreduciblePolynomialsFinder));

            Initialize(order, analysisResult.First().Key);
            IrreduciblePolynomial = irreduciblePolynomialsFinder.Find(Characteristic, analysisResult.First().Value);
            InitializeAdditionalStructures();
        }

        public PrimePowerOrderField(int order, Polynomial irreduciblePolynomial)
        {
            var analysisResult = AnalyzeOrder(order);
            if (analysisResult.Count != 1)
                throw new ArgumentException("Field order isn't a prime number power");
            if (analysisResult.First().Value == 1)
                throw new ArgumentException("Field order is a prime number");

            if (irreduciblePolynomial == null)
                throw new ArgumentNullException(nameof(irreduciblePolynomial));
            if(irreduciblePolynomial.Field.Order != analysisResult.First().Key)
                throw new ArgumentException("Irreducible polynomial isn't declared over properly field");
            if (irreduciblePolynomial.Degree != analysisResult.First().Value)
                throw new ArgumentException("Irreducible polynomial degree isn't correct");
            if (Enumerable.Range(0, analysisResult.First().Key).Any(x => irreduciblePolynomial.Evaluate(x) == 0))
                throw new ArgumentException($"Polynomial {irreduciblePolynomial} isn't irreducible");

            Initialize(order, analysisResult.First().Key);
            IrreduciblePolynomial = irreduciblePolynomial;
            InitializeAdditionalStructures();
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

        public override string ToString()
        {
            return $"GF{Order}, irreducible polynomial {IrreduciblePolynomial}";
        }
    }
}