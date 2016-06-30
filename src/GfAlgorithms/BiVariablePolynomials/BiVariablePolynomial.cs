namespace GfAlgorithms.BiVariablePolynomials
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class BiVariablePolynomial : IEnumerable<KeyValuePair<Tuple<int, int>, FieldElement>>
    {
        /// <summary>
        /// Поле, над которым построен многочлен
        /// </summary>
        public GaloisField Field { get; }
        /// <summary>
        /// Коэффициенты многочлена
        /// </summary>
        private readonly Dictionary<Tuple<int, int>, FieldElement> _coefficients;

        private BiVariablePolynomial RemoveZeroCoefficients()
        {
            var coefficientsArray = _coefficients.ToArray();
            foreach (var coefficient in coefficientsArray)
                if (coefficient.Value.Representation == 0)
                    _coefficients.Remove(coefficient.Key);

            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public BiVariablePolynomial(GaloisField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Field = field;

            _coefficients = new Dictionary<Tuple<int, int>, FieldElement>();
        }

        public BiVariablePolynomial(BiVariablePolynomial polynomial)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            Field = polynomial.Field;
            _coefficients = polynomial._coefficients.ToDictionary(x => x.Key, x => new FieldElement(x.Value));
        }

        public FieldElement this[Tuple<int, int> monomial]
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

        public bool IsZero => _coefficients.Count == 0;
        public int MaxXDegree => IsZero ? 0 : _coefficients.Keys.Max(x => x.Item1);
        public int MaxYDegree => IsZero ? 0 : _coefficients.Keys.Max(x => x.Item2);

        public override string ToString()
        {
            var monomials = _coefficients
                .Where(x => x.Value.Representation != 0)
                .OrderBy(x => x.Key.Item2)
                .ThenBy(x => x.Key.Item1)
                .Select(x =>
                        {
                            var template = "{0}x^{1}y^{2}";

                            if (x.Key.Item1 == 0 && x.Key.Item2 == 0)
                                template = "{0}";
                            else
                            {
                                if (x.Value.Representation == 1)
                                    template = template.Replace("{0}", "");

                                if (x.Key.Item1 == 0)
                                    template = template.Replace("x^{1}", "");
                                if (x.Key.Item1 == 1)
                                    template = template.Replace("^{1}", "");

                                if (x.Key.Item2 == 0)
                                    template = template.Replace("y^{2}", "");
                                if (x.Key.Item2 == 1)
                                    template = template.Replace("^{2}", "");
                            }

                            return string.Format(template, x.Value.Representation, x.Key.Item1, x.Key.Item2);
                        });
            return string.Join((string) "+", (IEnumerable<string>) monomials);
        }

        public FieldElement Evaluate(FieldElement xValue, FieldElement yValue)
        {
            var result = Field.Zero();
            foreach (var coefficient in _coefficients)
                result += coefficient.Value
                          *FieldElement.Pow(xValue, coefficient.Key.Item1)
                          *FieldElement.Pow(yValue, coefficient.Key.Item2);

            return result;
        }

        public BiVariablePolynomial Add(BiVariablePolynomial b)
        {
            foreach (var otherCoefficient in b._coefficients)
            {
                FieldElement coeficientValue;
                if (_coefficients.TryGetValue(otherCoefficient.Key, out coeficientValue))
                    coeficientValue.Add(otherCoefficient.Value);
                else
                    _coefficients[otherCoefficient.Key] = otherCoefficient.Value;
            }

            return RemoveZeroCoefficients();
        }

        public BiVariablePolynomial Subtract(BiVariablePolynomial b)
        {
            foreach (var otherCoefficient in b._coefficients)
            {
                FieldElement coeficientValue;
                if (_coefficients.TryGetValue(otherCoefficient.Key, out coeficientValue) == false)
                {
                    coeficientValue = Field.Zero();
                    _coefficients[otherCoefficient.Key] = coeficientValue;
                }
                coeficientValue.Subtract(otherCoefficient.Value);
            }

            return RemoveZeroCoefficients();
        }

        public BiVariablePolynomial Multiply(BiVariablePolynomial b)
        {
            var coeficientsArray = _coefficients.ToArray();
            _coefficients.Clear();

            if (b.IsZero)
                return this;

            foreach (var coefficient in coeficientsArray)
                foreach (var otherCoefficient in b._coefficients)
                {
                    FieldElement coeficientValue;
                    var monomial = new Tuple<int, int>(coefficient.Key.Item1 + otherCoefficient.Key.Item1,
                        coefficient.Key.Item2 + otherCoefficient.Key.Item2);
                    
                    if (_coefficients.TryGetValue(monomial, out coeficientValue) == false)
                    {
                        coeficientValue = Field.Zero();
                        _coefficients[monomial] = coeficientValue;
                    }
                    coeficientValue.Add(coefficient.Value*otherCoefficient.Value);
                }

            return RemoveZeroCoefficients();
        }

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

        private bool Equals(BiVariablePolynomial other)
        {
            return _coefficients.OrderBy(x => x.Key).SequenceEqual(other._coefficients.OrderBy(x => x.Key))
                   && Field.Equals(other.Field);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BiVariablePolynomial) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_coefficients.Aggregate(0, (hash, x) => hash*31 ^ x.GetHashCode())*397) ^ Field.GetHashCode();
            }
        }

        public IEnumerator<KeyValuePair<Tuple<int, int>, FieldElement>> GetEnumerator()
        {
            return _coefficients.GetEnumerator();
        }

        public static BiVariablePolynomial Add(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Add(b);
        }

        public static BiVariablePolynomial Subtract(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Subtract(b);
        }

        public static BiVariablePolynomial Multiply(BiVariablePolynomial a, BiVariablePolynomial b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Multiply(b);
        }

        public static BiVariablePolynomial Multiply(BiVariablePolynomial a, FieldElement b)
        {
            var c = new BiVariablePolynomial(a);
            return c.Multiply(b);
        }

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