namespace AppliedAlgebra.GfAlgorithms.BiVariablePolynomials
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Class represents a polynomial with two indeterminates over a finite field
    /// </summary>
    public class BiVariablePolynomial : IEnumerable<KeyValuePair<(int xDegree, int yDegree), FieldElement>>
    {
        /// <summary>
        /// Field from which the coefficients of the polynomial
        /// </summary>
        public GaloisField Field { get; }
        /// <summary>
        /// Coefficients of the polynomial indexed by their monomials
        /// </summary>
        private readonly Dictionary<(int xDegree, int yDegree), FieldElement> _coefficients;

        /// <summary>
        /// Method for removing polynomial's zero coefficients
        /// </summary>
        /// <returns>Modified polynomial</returns>
        private BiVariablePolynomial RemoveZeroCoefficients()
        {
            var coefficientsArray = _coefficients.ToArray();
            foreach (var coefficient in coefficientsArray)
                if (coefficient.Value.Representation == 0)
                    _coefficients.Remove(coefficient.Key);

            return this;
        }

        /// <summary>
        /// Realisation of the member of IEnumerable interface for enumerating polynomial's coefficients
        /// </summary>
        /// <returns>Created enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Constructor for creation zero bivariate polynomial over field <paramref name="field"/> with estimated count of coefficients <paramref name="estimatedCoefficientsCount"/>
        /// </summary>
        /// <param name="field">Field from which the coefficients of the bivariate polynomial</param>
        /// <param name="estimatedCoefficientsCount">Estimated count of the bivariate polynomial's coefficients</param>
        public BiVariablePolynomial(GaloisField field, int? estimatedCoefficientsCount = null)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Field = field;

            _coefficients = new Dictionary<(int xDegree, int yDegree), FieldElement>(estimatedCoefficientsCount ?? 0);
        }

        /// <summary>
        /// Constructor for creating copy of the bivariate polynomial <paramref name="polynomial"/>
        /// </summary>
        /// <param name="polynomial">Copied bivariate polynomial</param>
        public BiVariablePolynomial(BiVariablePolynomial polynomial)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            Field = polynomial.Field;

            _coefficients = polynomial._coefficients.ToDictionary(x => x.Key, x => new FieldElement(x.Value));
        }

        /// <summary>
        /// Indexing property for obtain bivariate polynomial coefficient by <paramref name="monomial"/>
        /// </summary>
        /// <param name="monomial">Monomial which coefficient is required</param>
        /// <returns>Bivariate polynomial coefficient</returns>
        public FieldElement this[(int xDegree, int yDegree) monomial]
        {
            get
            {
                FieldElement coefficient;
                if (_coefficients.TryGetValue(monomial, out coefficient) == false)
                    coefficient = Field.Zero();
                return coefficient;
            }
            set
            {
                if (Field.Equals(value.Field) == false)
                    throw new ArgumentException("Incorrect field");

                if (value.Representation != 0)
                    _coefficients[monomial] = value;
                else
                    _coefficients.Remove(monomial);
            }
        }

        /// <summary>
        /// Checks is polynomial zero
        /// </summary>
        public bool IsZero => _coefficients.Count == 0;
        /// <summary>
        /// Maximum degree of the current bivariate polynomial by x variable
        /// </summary>
        public int MaxXDegree => IsZero ? 0 : _coefficients.Keys.Max(x => x.xDegree);
        /// <summary>
        /// Maximum degree of the current bivariate polynomial by y variable
        /// </summary>
        public int MaxYDegree => IsZero ? 0 : _coefficients.Keys.Max(x => x.yDegree);
        /// <summary>
        /// Current polynomial's coefficients count
        /// </summary>
        public int CoefficientsCount => _coefficients.Count;

        /// <summary>
        /// Method for obtaining a string representation of the current bivariate polynomial
        /// </summary>
        /// <returns>String representation of the current bivariate polynomial</returns>
        public override string ToString()
        {
            var monomials = _coefficients
                .Where(x => x.Value.Representation != 0)
                .OrderBy(x => x.Key.yDegree)
                .ThenBy(x => x.Key.xDegree)
                .Select(x =>
                        {
                            var template = "{0}x^{1}y^{2}";

                            if (x.Key.xDegree == 0 && x.Key.yDegree == 0)
                                template = "{0}";
                            else
                            {
                                if (x.Value.Representation == 1)
                                    template = template.Replace("{0}", "");

                                if (x.Key.xDegree == 0)
                                    template = template.Replace("x^{1}", "");
                                if (x.Key.xDegree == 1)
                                    template = template.Replace("^{1}", "");

                                if (x.Key.yDegree == 0)
                                    template = template.Replace("y^{2}", "");
                                if (x.Key.yDegree == 1)
                                    template = template.Replace("^{2}", "");
                            }

                            return string.Format(template, x.Value.Representation, x.Key.xDegree, x.Key.yDegree);
                        });
            return string.Join("+", monomials);
        }

        /// <summary>
        /// Method for calculating bivariate polynomial value for variable x value <paramref name="xValue"/> and variable y value <paramref name="yValue"/>
        /// </summary>
        /// <param name="xValue">x variable value</param>
        /// <param name="yValue">y variable value</param>
        /// <returns>Bivariate polynomial value</returns>
        public FieldElement Evaluate(FieldElement xValue, FieldElement yValue)
        {
            var result = Field.Zero();
            foreach (var coefficient in _coefficients)
                result += coefficient.Value
                          *FieldElement.Pow(xValue, coefficient.Key.xDegree)
                          *FieldElement.Pow(yValue, coefficient.Key.yDegree);

            return result;
        }

        /// <summary>
        /// Method for adding bivariate polynomial <paramref name="other"/> multiplied by <paramref name="b"/> to current bivariate polynomial
        /// </summary>
        /// <param name="b">Term multiplier</param>
        /// <param name="other">Term</param>
        public BiVariablePolynomial Add(FieldElement b, BiVariablePolynomial other)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException(nameof(b));
            if (Field.Equals(other.Field) == false)
                throw new ArgumentException(nameof(other));

            if (b.Representation == 0)
                return this;

            foreach (var otherCoefficient in other._coefficients)
            {
                FieldElement coeficientValue;
                if (_coefficients.TryGetValue(otherCoefficient.Key, out coeficientValue))
                {
                    coeficientValue.Add(b*otherCoefficient.Value);
                    if (coeficientValue.Representation == 0)
                        _coefficients.Remove(otherCoefficient.Key);
                }
                else
                    _coefficients[otherCoefficient.Key] = b*otherCoefficient.Value;
            }

            return this;
        }

        /// <summary>
        /// Method for adding bivariate polynomial <paramref name="other"/> to current bivariate polynomial
        /// </summary>
        /// <param name="other">Term</param>
        public BiVariablePolynomial Add(BiVariablePolynomial other)
        {
            return Add(Field.One(), other);
        }

        /// <summary>
        /// Method for subtracting bivariate polynomial <paramref name="other"/> multiplied by <paramref name="b"/> from current bivariate polynomial
        /// </summary>
        /// <param name="b">Subtrahend multiplier</param>
        /// <param name="other">Subtrahend</param>
        public BiVariablePolynomial Subtract(FieldElement b, BiVariablePolynomial other)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException(nameof(b));
            if (Field.Equals(other.Field) == false)
                throw new ArgumentException(nameof(other));

            if (b.Representation == 0)
                return this;

            foreach (var otherCoefficient in other._coefficients)
            {
                FieldElement coeficientValue;
                if (_coefficients.TryGetValue(otherCoefficient.Key, out coeficientValue) == false)
                {
                    coeficientValue = Field.Zero();
                    _coefficients[otherCoefficient.Key] = coeficientValue;
                }

                coeficientValue.Subtract(b*otherCoefficient.Value);
                if (coeficientValue.Representation == 0)
                    _coefficients.Remove(otherCoefficient.Key);
            }

            return this;
        }

        /// <summary>
        /// Method for subtracting bivariate polynomial <paramref name="other"/> from current bivariate polynomial
        /// </summary>
        /// <param name="other">Subtrahend</param>
        public BiVariablePolynomial Subtract(BiVariablePolynomial other)
        {
            return Subtract(Field.One(), other);
        }

        /// <summary>
        /// Method for multiplying current bivariate polynomial by bivariate polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="b">Factor</param>
        public BiVariablePolynomial Multiply(BiVariablePolynomial b)
        {
            var coefficientsArray = _coefficients.ToArray();
            _coefficients.Clear();

            if (b.IsZero)
                return this;

            foreach (var coefficient in coefficientsArray)
                foreach (var otherCoefficient in b._coefficients)
                {
                    FieldElement coefficientValue;
                    var monomial = (
                        coefficient.Key.xDegree + otherCoefficient.Key.xDegree,
                        coefficient.Key.yDegree + otherCoefficient.Key.yDegree
                    );
                    
                    if (_coefficients.TryGetValue(monomial, out coefficientValue) == false)
                    {
                        coefficientValue = Field.Zero();
                        _coefficients[monomial] = coefficientValue;
                    }
                    coefficientValue.Add(coefficient.Value*otherCoefficient.Value);
                }

            return RemoveZeroCoefficients();
        }

        /// <summary>
        /// Method for multiplying current bivariate polynomial by field elemnt <paramref name="b"/>
        /// </summary>
        /// <param name="b">Factor</param>
        public BiVariablePolynomial Multiply(FieldElement b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException(nameof(b));

            if (b.Representation == 0)
                _coefficients.Clear();
            else
                foreach (var coefficient in _coefficients.Values)
                    coefficient.Multiply(b);

            return this;
        }

        /// <summary>
        /// Method for checking the equality of the current bivariate polynomial to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another bivariate polynomial</param>
        /// <returns>Checking result</returns>
        private bool Equals(BiVariablePolynomial other)
        {
            return _coefficients.OrderBy(x => x.Key).SequenceEqual(other._coefficients.OrderBy(x => x.Key))
                   && Field.Equals(other.Field);
        }

        /// <summary>
        /// Method for checking the equality of the current bivariate polynomial to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>Checking result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BiVariablePolynomial) obj);
        }

        /// <summary>
        /// Method for calculation object hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (_coefficients.Aggregate(0, (hash, x) => hash*31 ^ x.GetHashCode())*397) ^ Field.GetHashCode();
            }
        }

        /// <summary>
        /// Method for getting enumerator for the current bivariate polynomial coefficients enumeration
        /// </summary>
        /// <returns>Current bivariate polynomial coefficients enumerator</returns>
        public IEnumerator<KeyValuePair<(int xDegree, int yDegree), FieldElement>> GetEnumerator()
        {
            return _coefficients.GetEnumerator();
        }

        /// <summary>
        /// Method for adding bivariate polynomial <paramref name="b"/> to <paramref name="a"/>
        /// </summary>
        /// <param name="a">First term</param>
        /// <param name="b">Second term</param>
        public static BiVariablePolynomial Add(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Add(b);
        }

        /// <summary>
        /// Method for subtracting bivariate polynomial <paramref name="b"/> from <paramref name="a"/>
        /// </summary>
        /// <param name="a">Minuend</param>
        /// <param name="b">Subtrahend</param>
        public static BiVariablePolynomial Subtract(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Subtract(b);
        }

        /// <summary>
        /// Method for multiplying bivariate polynomial <paramref name="a"/> by bivariate polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static BiVariablePolynomial Multiply(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Multiply(b);
        }

        /// <summary>
        /// Method for multiplying bivariate polynomial <paramref name="a"/> by field elemnt <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static BiVariablePolynomial Multiply(BiVariablePolynomial a, FieldElement b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Multiply(b);
        }

        /// <summary>
        /// Method for multiplying field elemnt <paramref name="a"/> by bivariate polynomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static BiVariablePolynomial Multiply(FieldElement a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(b);
            return c.Multiply(a);
        }

        public static BiVariablePolynomial operator +(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            return Add(a, b);
        }

        public static BiVariablePolynomial operator -(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            return Subtract(a, b);
        }

        public static BiVariablePolynomial operator *(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            return Multiply(a, b);
        }

        public static BiVariablePolynomial operator *(BiVariablePolynomial a, FieldElement b)
        {
            return Multiply(a, b);
        }

        public static BiVariablePolynomial operator *(FieldElement a, BiVariablePolynomial b)
        {
            return Multiply(a, b);
        }
    }
}