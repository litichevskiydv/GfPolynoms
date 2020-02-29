namespace AppliedAlgebra.GfPolynoms.GaloisFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IrreduciblePolynomialsFinder;

    /// <summary>
    /// Field in which order is the power of prime number
    /// </summary>
    public class PrimePowerOrderField : GaloisField
    {
        /// <summary>
        /// Polynomial representations of current field elements, ordered by their number representations
        /// </summary>
        private Polynomial[] _polynomialByRepresentation;
        /// <summary>
        /// Number representations of current field elements, indexed by their polynomial representations
        /// </summary>
        private Dictionary<Polynomial, int> _representationByPolynomial;

        /// <summary>
        /// Results of addition operation between all field elements
        /// </summary>
        private int[][] _additionResults;
        /// <summary>
        /// Results of subtraction operation between all field elements
        /// </summary>
        private int[][] _subtractionResults;

        /// <summary>
        /// Polynomial which have no zeros under field with order <see cref="GaloisField.Characteristic"/>
        /// </summary>
        public Polynomial IrreduciblePolynomial { get; }

        /// <summary>
        /// Method for calculating field element number representation by its polynomial representation
        /// </summary>
        /// <param name="characteristic">Field characteristic</param>
        /// <param name="coefficients">Field element's polynomial representation coefficients</param>
        /// <returns></returns>
        private static int CalculateElementRepresentation(int characteristic, IEnumerable<int> coefficients)
        {
            return coefficients.Aggregate(0, (current, coefficient) => current*characteristic + coefficient);
        }

        /// <summary>
        /// Method for calculating polynomial representation for all filed's elements
        /// </summary>
        /// <param name="characteristic">Field characteristic</param>
        /// <param name="coefficients">Array for storing generated coefficients of field element</param>
        /// <param name="position">Processed coefficient index</param>
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
        /// Method for calculating elements of field multiplicative group
        /// </summary>
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

        /// <summary>
        /// Меthod for calculating results of addition operation between all field elements
        /// </summary>
        private void PrecalculateAdditionResults()
        {
            for (var i = 0; i < Order; i++)
            {
                _additionResults[i] = new int[Order];
                for (var j = 0; j < Order; j++)
                    _additionResults[i][j] = _representationByPolynomial[_polynomialByRepresentation[i] + _polynomialByRepresentation[j]];
            }
        }

        /// <summary>
        /// Меthod for calculating results of subtraction operation between all field elements
        /// </summary>
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
        /// Method for initializing field additional structures
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
        /// Constructor for creating field with order <paramref name="order"/>
        /// </summary>
        /// <param name="order">Field order</param>
        public PrimePowerOrderField(int order) : this(order, new SimpleFinder())
        {
        }

        /// <summary>
        /// Constructor for creating field with order <paramref name="order"/> and irreducible polynomials generator <paramref name="irreduciblePolynomialsFinder"/>
        /// </summary>
        /// <param name="order">Field order</param>
        /// <param name="irreduciblePolynomialsFinder">Irreducible polynomials generator </param>
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

        /// <summary>
        /// Constructor for creating field with order <paramref name="order"/> and irreducible polynomial <paramref name="irreduciblePolynomial"/>
        /// </summary>
        /// <param name="order">Field order</param>
        /// <param name="irreduciblePolynomial">Field irreducible polynomial</param>
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

        /// <summary>
        /// Method for checking the equality of the current field to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another field</param>
        /// <returns>Checking result</returns>
        private bool Equals(PrimePowerOrderField other)
        {
            return IrreduciblePolynomial.Equals(other.IrreduciblePolynomial) && Order == other.Order;
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
            return Equals((PrimePowerOrderField) obj);
        }

        /// <summary>
        /// Method for calculation object hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (IrreduciblePolynomial.GetHashCode()*397) ^ Order;
            }
        }

        /// <summary>
        /// Indexing property for obtain field element polynomial representation by its number representation
        /// </summary>
        /// <param name="index">field element number representation</param>
        /// <returns>Field element polynomial representation</returns>
        public Polynomial this[int index]
        {
            get
            {
                if (IsFieldElement(index) == false)
                    throw new IndexOutOfRangeException();

                return _polynomialByRepresentation[index];
            }
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

            return _additionResults[a][b];
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

            return _subtractionResults[a][b];
        }

        /// <summary>
        /// Inverts field element
        /// </summary>
        /// <param name="a">Invetible element</param>
        /// <returns>Inverse element</returns>
        public override int InverseForAddition(int a)
        {
            if (IsFieldElement(a) == false)
                throw new ArgumentException($"Element {a} is not field member");

            return _subtractionResults[0][a];
        }

        /// <summary>
        /// Method for obtaining a string representation of the current field
        /// </summary>
        /// <returns>String representation of the current field</returns>
        public override string ToString()
        {
            return $"GF{Order}, irreducible polynomial {IrreduciblePolynomial}";
        }
    }
}