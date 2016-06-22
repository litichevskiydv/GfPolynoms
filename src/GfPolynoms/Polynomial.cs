namespace GfPolynoms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaloisFields;

    /// <summary>
    /// Класс многочленов над некоторым конечным полемя
    /// </summary>
    public class Polynomial
    {
        /// <summary>
        /// Коэффициенты многочлена
        /// </summary>
        private List<int> _coefficients;
        /// <summary>
        /// Поле, над которым построен многочлен
        /// </summary>
        public GaloisField Field { get; }

        /// <summary>
        /// Операция усечения ведущих нулей коэфициентов многочлена
        /// </summary>
        /// <returns>Текущий многочлен без нулевых ведущих коэфициентов</returns>
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
        ///     Увеличивет степень многочлена до заданной, заполняя список коэффициентов нулями
        /// </summary>
        /// <param name="newDegree">Новая степень многочлена</param>
        private void Enlarge(int newDegree)
        {
            if (Degree < newDegree)
                _coefficients.AddRange(Enumerable.Repeat(0, newDegree - Degree));
        }

        private bool Equals(Polynomial other)
        {
            return _coefficients.SequenceEqual(other._coefficients) && Equals(Field, other.Field);
        }

        /// <summary>
        ///     Конструктор, создающий нулевой многочлен над некоторым полем
        /// </summary>
        /// <param name="field">Поле, к которому принадлежат коэффициенты</param>
        public Polynomial(GaloisField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Field = field;
            _coefficients = new List<int> {0};
        }

        /// <summary>
        ///     Конструктор, создающий многочлен с переданными коэффициентами над некторомы полем
        /// </summary>
        /// <param name="field">Поле, к которому принадлежат коэффициенты</param>
        /// <param name="coefficients">Коэффициенты создаваемого многочлена</param>
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
        ///     Создает копию переданного многочлена
        /// </summary>
        /// <param name="polynomial">Копируемый многочлен</param>
        public Polynomial(Polynomial polynomial)
        {
            Field = polynomial.Field;
            _coefficients = polynomial._coefficients.ToList();
        }

        public int Degree => _coefficients.Count - 1;

        public override string ToString()
        {
            var monomials = _coefficients
                .Select((x, i) => new Tuple<int, int>(x, i))
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Polynomial)obj);
        }

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
        /// Добавляет переданный многочлен к текущему
        /// </summary>
        /// <param name="b">Добавляемый многочлен</param>
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
        /// Вычетает переданный многочлен из текущего
        /// </summary>
        /// <param name="b">Вычетаемый многочлен</param>
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
        /// Умножает переданный многочлен на число
        /// </summary>
        /// <param name="b">Коэффициент для домножения</param>
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
        /// Умножает текущий многочлен на переданный
        /// </summary>
        /// <param name="b">Многочлен для домножения</param>
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
        /// Делит текущий многочлен на элемент поля
        /// </summary>
        /// <param name="b">Делитель</param>
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
        /// Вычисление результата деления текущего многочлена на заданный
        /// </summary>
        /// <param name="b">Делитель</param>
        /// <returns>Пара (частное, остаток)</returns>
        public Tuple<Polynomial, Polynomial> DivideEx(Polynomial b)
        {
            if (Field.Equals(b.Field) == false || b.IsZero)
                throw new ArgumentException(nameof(b));

            var result = new Tuple<Polynomial, Polynomial>(new Polynomial(Field), new Polynomial(this));
            var quotientPolynomial = result.Item1;
            var remainderPolynomial = result.Item2;

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
        /// Делит текущий многочлен на заданный
        /// </summary>
        /// <param name="b">Делитель</param>
        public Polynomial Divide(Polynomial b)
        {
            var divisionResult = DivideEx(b);
            _coefficients = divisionResult.Item1._coefficients;
            return this;
        }

        /// <summary>
        /// Вычисляет текущий многочлен по модулю переданного
        /// </summary>
        /// <param name="b">Многочлен, по модулю которого берется текущий</param>
        public Polynomial Modulo(Polynomial b)
        {
            var divisionResult = DivideEx(b);
            _coefficients = divisionResult.Item2._coefficients;
            return this;
        }

        /// <summary>
        /// Сдвиг текущего многочлена на указанное количество разрядов вправо
        /// </summary>
        /// <param name="degreeDelta">Количество разрядов, на которые сдвигается многочлен</param>
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
        /// Вычисляет значение многочлена для заданного значения переменной
        /// </summary>
        /// <param name="variableValue">Значение переменной</param>
        /// <returns>Значение многочлена</returns>
        public int Evaluate(int variableValue)
        {
            if(Field.IsFieldElement(variableValue) == false)
                throw new ArgumentException(nameof(variableValue));

            var result = 0;
            for (var i = _coefficients.Count - 1; i >= 0; i--)
                result = Field.Add(_coefficients[i], Field.Multiply(result, variableValue));
            return result;
        }

        public static Polynomial Add(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Add(b);
        }

        public static Polynomial Subtract(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Subtract(b);
        }

        public static Polynomial Multiply(Polynomial a, int b)
        {
            var c = new Polynomial(a);
            return c.Multiply(b);
        }

        public static Polynomial Multiply(int a, Polynomial b)
        {
            return Multiply(b, a);
        }

        public static Polynomial Multiply(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Multiply(b);
        }

        public static Polynomial Modulo(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Modulo(b);
        }

        public static Polynomial Divide(Polynomial a, Polynomial b)
        {
            var c = new Polynomial(a);
            return c.Divide(b);
        }

        public static Polynomial Divide(Polynomial a, int b)
        {
            var c = new Polynomial(a);
            return c.Divide(b);
        }

        public static Polynomial Divide(int a, Polynomial b)
        {
            return Divide(b, a);
        }

        public static Polynomial RightShift(Polynomial a, int degreeDelta)
        {
            var c = new Polynomial(a);
            return c.RightShift(degreeDelta);
        }

        public static Polynomial operator +(Polynomial a, Polynomial b)
        {
            return Add(a, b);
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

        public static Polynomial operator /(int a, Polynomial b)
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