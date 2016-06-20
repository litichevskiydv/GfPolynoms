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
        public IGaluaField Field { get; }
        /// <summary>
        /// Коэффициенты многочлена
        /// </summary>
        private readonly Dictionary<Tuple<int, int>, FieldElement> _coefficients;
        /// <summary>
        /// Веса взвешенной степени
        /// </summary>
        private Tuple<int, int> DegreeWeight { get; }
        /// <summary>
        /// Максимальная взвешенная степень многочлена с весами <see cref="DegreeWeight"/>
        /// </summary>
        private int MaxWeightedDegree { get; }

        /// <summary>
        /// Максимальная степень переменной x для максимальной взвешенной степени <see cref="MaxWeightedDegree"/> с весами <see cref="DegreeWeight"/>
        /// </summary>
        public int MaxXDegree { get; }

        /// <summary>
        /// Максимальная степень переменной y для максимальной взвешенной степени <see cref="MaxWeightedDegree"/> с весами <see cref="DegreeWeight"/>
        /// </summary>
        public int MaxYDegree { get; }

        /// <summary>
        /// Метод для проверки, существуют ли в полиноме указанные степени
        /// </summary>
        /// <param name="xDegree">Степень переменной X</param>
        /// <param name="yDegree">Степень переменной Y</param>
        private void ValidateMonomial(int xDegree, int yDegree)
        {
            if (xDegree < 0 || xDegree > MaxXDegree)
                throw new ArgumentException(nameof(xDegree));
            if (yDegree < 0 || yDegree > MaxYDegree)
                throw new ArgumentException(nameof(yDegree));
            if (xDegree * DegreeWeight.Item1 + yDegree * DegreeWeight.Item2 > MaxWeightedDegree)
                throw new ArgumentException("Such monomial doesn't exist");
        }

        private int GetMonomialPlainIndex(Tuple<int, int> monomial)
        {
            return monomial.Item1 + monomial.Item2 * (MaxXDegree + 1);
        }

        public BiVariablePolynomial(IGaluaField field, Tuple<int, int> degreeWeight, int maxWeightedDegree)
        {
            if(degreeWeight == null)
                throw new ArgumentNullException(nameof(degreeWeight));
            if (degreeWeight.Item1 < 0 || degreeWeight.Item2 < 0
                || degreeWeight.Item1 == 0 && degreeWeight.Item2 == 0)
                throw new ArgumentException(nameof(degreeWeight));
            if (maxWeightedDegree <= 0)
                throw new ArgumentException(nameof(maxWeightedDegree));

            Field = field;
            DegreeWeight = degreeWeight;
            MaxWeightedDegree = maxWeightedDegree;

            _coefficients = new Dictionary<Tuple<int, int>, FieldElement>();
            MaxXDegree =  MaxWeightedDegree/DegreeWeight.Item1;
            MaxYDegree = MaxWeightedDegree/DegreeWeight.Item2;
        }

        public FieldElement this[Tuple<int, int> monomial]
        {
            get
            {
                ValidateMonomial(monomial.Item1, monomial.Item2);

                FieldElement coefficient;
                if (_coefficients.TryGetValue(monomial, out coefficient) == false)
                    coefficient = Field.Zero();
                return coefficient;
            }
            set
            {
                ValidateMonomial(monomial.Item1, monomial.Item2);
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
                .OrderByDescending(x => GetMonomialPlainIndex(x.Key))
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
    }
}