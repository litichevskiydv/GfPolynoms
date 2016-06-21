namespace RsListDecoding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaluaFields;

    public class BiVariablePolynomial
    {
        /// <summary>
        /// Поле, над которым построен многочлен
        /// </summary>
        public GaluaField Field { get; }
        /// <summary>
        /// Коэффициенты многочлена
        /// </summary>
        private readonly Dictionary<Tuple<int, int>, FieldElement> _coefficients;

        public BiVariablePolynomial(GaluaField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Field = field;

            _coefficients = new Dictionary<Tuple<int, int>, FieldElement>();
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
            }
        }

        public bool IsZero => _coefficients.Count == 0;

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
            return string.Join("+", monomials);
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
    }
}