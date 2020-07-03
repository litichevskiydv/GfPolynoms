namespace AppliedAlgebra.GfPolynoms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaloisFields;

    /// <summary>
    /// Polynomial over a finite field
    /// </summary>
    public class Polynomial
    {
        /// <summary>
        /// Result of polynomial division
        /// </summary>
        public class DevisionResult
        {
            public Polynomial Quotient { get; }

            public Polynomial Remainder { get; }

            public DevisionResult(Polynomial quotient, Polynomial remainder)
            {
                if (quotient == null)
                    throw new ArgumentNullException(nameof(quotient));
                if (remainder == null)
                    throw new ArgumentNullException(nameof(remainder));

                Quotient = quotient;
                Remainder = remainder;
            }
        }

        /// <summary>
        /// Coefficients of the polynomial
        /// </summary>
        private List<int> _coefficients;
        /// <summary>
        /// Field from which the coefficients of the polynomial
        /// </summary>
        public GaloisField Field { get; }

        /// <summary>
        /// Method for truncating the leading zero coefficients of the polynomial
        /// </summary>
        /// <returns>Current polynomial</returns>
        private Polynomial Truncate()
        {
            int i;
            for (i = Degree; i >= 0 && _coefficients[i] == 0; i--)
            {
            }
            _coefficients = _coefficients.Take(i + 1).ToList();

            if (_coefficients.Count == 0)
                _coefficients.Add(0);
            return this;
        }

        /// <summary>
        /// Method for adding leading zeros to polynomial coefficients
        /// </summary>
        /// <param name="newDegree">A new degree of the polynomial</param>
        private void Enlarge(int newDegree)
        {
            if (Degree < newDegree)
                _coefficients.AddRange(Enumerable.Repeat(0, newDegree - Degree));
        }

        /// <summary>
        /// Method for checking the equality of the current polynomial to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another polynomial</param>
        /// <returns>Checking result</returns>
        private bool Equals(Polynomial other)
        {
            return _coefficients.SequenceEqual(other._coefficients) && Equals(Field, other.Field);
        }

        /// <summary>
        /// Constructor for creation zero polynomial over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the coefficients of the polynomial</param>
        public Polynomial(GaloisField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Field = field;
            _coefficients = new List<int> {0};
        }

        /// <summary>
        /// Constructor for creation polynomial over field <paramref name="field"/> with coefficients <paramref name="coefficients"/>
        /// </summary>
        /// <param name="field">Field from which the coefficients of the polynomial</param>
        /// <param name="coefficients">Coefficients of the new polynomial</param>
        public Polynomial(GaloisField field, params int[] coefficients)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (coefficients == null)
                throw new ArgumentNullException(nameof(coefficients));

            Field = field;

            _coefficients = coefficients.ToList();
            if (_coefficients.Any(x => Field.IsFieldElement(x) == false))
                throw new ArgumentException(nameof(coefficients));

            Truncate();
        }

        /// <summary>
        /// Constructor for creation polynomial with coefficients <paramref name="coefficients"/>
        /// </summary>
        /// <param name="coefficients">Coefficients of the new polynomial</param>
        public Polynomial(params FieldElement[] coefficients)
        {
            if(coefficients == null)
                throw new ArgumentNullException(nameof(coefficients));
            if (coefficients.Length == 0)
                throw new ArgumentException("At least one coefficient must be passed");
            if (coefficients.Select(x => x.Field).Distinct().Count() > 1)
                throw new ArgumentException("All coefficients must belong to the same field");

            Field = coefficients[0].Field;
            _coefficients = coefficients.Select(x => x.Representation).ToList();

            Truncate();
        }

        /// <summary>
        /// Constructor for creating copy of the polynomial <paramref name="polynomial"/>
        /// </summary>
        /// <param name="polynomial">The copied polynomial</param>
        public Polynomial(Polynomial polynomial)
        {
            Field = polynomial.Field;
            _coefficients = polynomial._coefficients.ToList();
        }

        /// <summary>
        /// Degree of the polynomial
        /// </summary>
        public int Degree => _coefficients.Count - 1;

        /// <summary>
        /// Method for obtaining a string representation of the current polynomial
        /// </summary>
        /// <returns>String representation of the current polynomial</returns>
        public override string ToString()
        {
            var monomials = _coefficients
                .Select((x, i) => Tuple.Create(x, i))
                .Where(x => x.Item1 != 0)
                .Select(x =>
                        {
                            var template = "{0}x^{1}";
                            if (x.Item2 == 0)
                                template = "{0}";
                            else
                            {
                                if (x.Item1 == 1)
                                    template = template.Replace("{0}", "");
                                if (x.Item2 == 1)
                                    template = template.Replace("^{1}", "");
                            }
                            return string.Format(template, x.Item1, x.Item2);
                        });
            return string.Join("+", monomials);
        }

        /// <summary>
        /// Method for checking the equality of the current polynomial to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>Checking result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Polynomial)obj);
        }

        /// <summary>
        /// Method for calculation object hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (_coefficients.Aggregate(0, (hash, x) => hash*31 ^ x)*397) ^ Field.GetHashCode();
            }
        }

        /// <summary>
        /// Checks is polynomial zero
        /// </summary>
        public bool IsZero => Degree == 0 && _coefficients[0] == 0;

        /// <summary>
        /// Indexing property for obtain bivariate polynomial coefficient by variable power <paramref name="index"/>
        /// </summary>
        /// <param name="index">Variable power which coefficient is required</param>
        /// <returns>Polynomial coefficient</returns>
        public int this[int index]
        {
            get
            {
                if (index < 0 || index > Degree)
                    throw new IndexOutOfRangeException();

                return _coefficients[index];
            }
            set
            {
                if (index < 0 || index > Degree)
                    throw new IndexOutOfRangeException();
                if (Field.IsFieldElement(value) == false)
                    throw new ArgumentException(nameof(value));

                _coefficients[index] = value;
            }
        }

        /// <summary>
        /// Method for adding polynomial <paramref name="b"/> to current polynomial
        /// </summary>
        /// <param name="b">Term</param>
        public Polynomial Add(Polynomial b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException(nameof(b));

            Enlarge(b.Degree);
            for (var i = 0; i <= b.Degree; i++)
                _coefficients[i] = Field.Add(_coefficients[i], b[i]);
            return Truncate();
        }

        /// <summary>
        /// Method for subtracting polynomial <paramref name="b"/> from current
        /// </summary>
        /// <param name="b">Subtrahend</param>
        public Polynomial Subtract(Polynomial b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException(nameof(b));

            Enlarge(b.Degree);
            for (var i = 0; i <= b.Degree; i++)
                _coefficients[i] = Field.Subtract(_coefficients[i], b[i]);
            return Truncate();
        }

        /// <summary>
        /// Method for multiplying current polynomial by number <paramref name="b"/>
        /// </summary>
        /// <param name="b">Factor</param>
        public Polynomial Multiply(int b)
        {
            if(Field.IsFieldElement(b) == false)
                throw new ArgumentException(nameof(b));

            if (IsZero || b == 0)
            {
                _coefficients = new List<int> { 0 };
                return this;
            }

            for (var i = 0; i <= Degree; i++)
                _coefficients[i] = Field.Multiply(_coefficients[i], b);
            return Truncate();
        }

        /// <summary>
        /// Method for multiplying current polynomial by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="b">Factor</param>
        public Polynomial Multiply(Polynomial b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException(nameof(b));

            if (Degree == 0 && _coefficients[0] == 0 || b.Degree == 0 && b[0] == 0)
            {
                _coefficients = new List<int> {0};
                return this;
            }

            var newCoefficients = new List<int>(Enumerable.Repeat(0, Degree + b.Degree + 1));
            for (var i = 0; i <= Degree; i++)
                for (var j = 0; j <= b.Degree; j++)
                    newCoefficients[i + j] = Field.Add(newCoefficients[i + j], Field.Multiply(_coefficients[i], b[j]));
            _coefficients = newCoefficients;
            return Truncate();
        }

        /// <summary>
        /// Method for dividing current polynomial by number <paramref name="b"/>
        /// </summary>
        /// <param name="b">Divider</param>
        public Polynomial Divide(int b)
        {
            if (Field.IsFieldElement(b) == false || b == 0)
                throw new ArgumentException(nameof(b));

            if (IsZero)
            {
                _coefficients = new List<int> { 0 };
                return this;
            }

            for (var i = 0; i <= Degree; i++)
                _coefficients[i] = Field.Divide(_coefficients[i], b);
            return Truncate();
        }

        /// <summary>
        /// Method for dividing current polynomial by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="b">Divider</param>
        /// <returns>Division result (quotient and remainder)</returns>
        public DevisionResult DivideExtended(Polynomial b)
        {
            if (Field.Equals(b.Field) == false || b.IsZero)
                throw new ArgumentException(nameof(b));

            var result = new DevisionResult(new Polynomial(Field), new Polynomial(this));
            var quotientPolynomial = result.Quotient;
            var remainderPolynomial = result.Remainder;

            if (Degree < b.Degree)
                return result;
            quotientPolynomial.Enlarge(Degree - b.Degree);

            for (var i = 0; i <= Degree - b.Degree; i++)
            {
                if (remainderPolynomial[Degree - i] == 0)
                    continue;

                var quotient = Field.Divide(remainderPolynomial[Degree - i], b[b.Degree]);
                quotientPolynomial[quotientPolynomial.Degree - i] = quotient;

                for (var j = 0; j <= b.Degree; j++)
                    remainderPolynomial[Degree - i - j] = Field.Subtract(remainderPolynomial[Degree - i - j],
                        Field.Multiply(b[b.Degree - j], quotient));
            }

            remainderPolynomial.Truncate();
            return result;
        }

        /// <summary>
        /// Method for dividing current polynomial by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="b">Divider</param>
        public Polynomial Divide(Polynomial b)
        {
            var divisionResult = DivideExtended(b);
            _coefficients = divisionResult.Quotient._coefficients;
            return this;
        }

        /// <summary>
        /// Method for calculating the remainder of dividing the current polynomial by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="b">Divider</param>
        public Polynomial Modulo(Polynomial b)
        {
            var divisionResult = DivideExtended(b);
            _coefficients = divisionResult.Remainder._coefficients;
            return this;
        }

        /// <summary>
        /// Method for shifting current polynomial by <paramref name="degreeDelta"/> positions to the right
        /// </summary>
        /// <param name="degreeDelta">Positions count for shifting</param>
        public Polynomial RightShift(int degreeDelta)
        {
            if (degreeDelta < 0)
                throw new ArgumentException(nameof(degreeDelta));
            if (degreeDelta == 0 || Degree == 0 && _coefficients[0] == 0)
                return this;

            var oldDegree = Degree;
            Enlarge(oldDegree + degreeDelta);

            for (var i = oldDegree; i >= 0; i--)
            {
                _coefficients[i + degreeDelta] = _coefficients[i];
                if (i < degreeDelta)
                    _coefficients[i] = 0;
            }
            return this;
        }

        /// <summary>
        /// Method for calculating polynomial value for variable value <paramref name="variableValue"/>
        /// </summary>
        /// <param name="variableValue">Variable value</param>
        /// <returns>Polynomial value</returns>
        public int Evaluate(int variableValue)
        {
            if(Field.IsFieldElement(variableValue) == false)
                throw new ArgumentException(nameof(variableValue));

            var result = 0;
            for (var i = _coefficients.Count - 1; i >= 0; i--)
                result = Field.Add(_coefficients[i], Field.Multiply(result, variableValue));
            return result;
        }

        /// <summary>
        /// Method for adding polynomial <paramref name="b"/> to <paramref name="a"/>
        /// </summary>
        /// <param name="a">First term</param>
        /// <param name="b">Second term</param>
        public static Polynomial Add(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Add(b);
        }

        /// <summary>
        /// Method for subtracting polynomial <paramref name="b"/> from <paramref name="a"/>
        /// </summary>
        /// <param name="a">Minuend</param>
        /// <param name="b">Subtrahend</param>
        public static Polynomial Subtract(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Subtract(b);
        }

        /// <summary>
        /// Method for multiplying polynomial <paramref name="a"/> by number <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static Polynomial Multiply(Polynomial a, int b)
        {
            var c = new Polynomial(a);
            return c.Multiply(b);
        }

        /// <summary>
        /// Method for multiplying number <paramref name="a"/> by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static Polynomial Multiply(int a, Polynomial b)
        {
            return Multiply(b, a);
        }

        /// <summary>
        /// Method for multiplying polynomial <paramref name="a"/> by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static Polynomial Multiply(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Multiply(b);
        }

        /// <summary>
        /// Method for calculating the remainder of dividing polynomial <paramref name="a"/> by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divider</param>
        public static Polynomial Modulo(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Modulo(b);
        }

        /// <summary>
        /// Method for calculating the quotient of dividing polynomial <paramref name="a"/> by polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divider</param>
        public static Polynomial Divide(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Divide(b);
        }

        /// <summary>
        /// Method for calculating the quotient of dividing polynomial <paramref name="a"/> by number <paramref name="b"/>
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divider</param>
        public static Polynomial Divide(Polynomial a, int b)
        {
            var c = new Polynomial(a);
            return c.Divide(b);
        }

        /// <summary>
        /// Method for shifting polynomial <paramref name="a"/> by <paramref name="degreeDelta"/> positions to the right
        /// </summary>
        /// <param name="a">Shifted polynomial</param>
        /// <param name="degreeDelta">Positions count for shifting</param>
        public static Polynomial RightShift(Polynomial a, int degreeDelta)
        {
            var c = new Polynomial(a);
            return c.RightShift(degreeDelta);
        }

        public static Polynomial operator +(Polynomial a, Polynomial b)
        {
            return Add(a, b);
        }

        public static Polynomial operator -(Polynomial a)
        {
            return new Polynomial(a.Field).Subtract(a);
        }

        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            return Subtract(a, b);
        }

        public static Polynomial operator *(Polynomial a, Polynomial b)
        {
            return Multiply(a, b);
        }

        public static Polynomial operator *(Polynomial a, int b)
        {
            return Multiply(a, b);
        }

        public static Polynomial operator *(int a, Polynomial b)
        {
            return Multiply(a, b);
        }

        public static Polynomial operator /(Polynomial a, Polynomial b)
        {
            return Divide(a, b);
        }

        public static Polynomial operator /(Polynomial a, int b)
        {
            return Divide(a, b);
        }

        public static Polynomial operator %(Polynomial a, Polynomial b)
        {
            return Modulo(a, b);
        }

        public static Polynomial operator >>(Polynomial a, int degreeDelta)
        {
            return RightShift(a, degreeDelta);
        }
    }
}