namespace GfPolynoms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaluaFields;

    /// <summary>
    /// Класс многочленов над некоторым конечным полемя
    /// </summary>
    public class Polynom
    {
        /// <summary>
        /// Коэффициенты многочлена
        /// </summary>
        private List<int> _coefficients;

        private bool Equals(Polynom other)
        {
            return _coefficients.SequenceEqual(other._coefficients) && Equals(Field, other.Field);
        }

        /// <summary>
        ///     Конструктор, создающий нулевой многочлен над некоторым полем
        /// </summary>
        /// <param name="field">Поле, к которому принадлежат коэффициенты</param>
        public Polynom(IGaluaField field)
        {
            if (field == null)
                throw new ArgumentException("field");

            Field = field;
            _coefficients = new List<int> {0};
        }

        /// <summary>
        ///     Конструктор, создающий многочлен с переданными коэффициентами над некторомы полем
        /// </summary>
        /// <param name="field">Поле, к которому принадлежат коэффициенты</param>
        /// <param name="coefficients">Коэффициенты создаваемого многочлена</param>
        public Polynom(IGaluaField field, IEnumerable<int> coefficients)
        {
            if (field == null)
                throw new ArgumentException("field");
            if (coefficients == null)
                throw new ArgumentException("coefficients");

            Field = field;

            _coefficients = coefficients.ToList();
            if (_coefficients.Any(x => Field.IsFieldElement(x) == false))
                throw new ArgumentException("coefficients");

            Truncate();
        }

        /// <summary>
        ///     Создает копию переданного многочлена
        /// </summary>
        /// <param name="polynom">Копируемый многочлен</param>
        public Polynom(Polynom polynom)
        {
            Field = polynom.Field;
            _coefficients = polynom._coefficients.ToList();
        }

        public IGaluaField Field { get; }
        public int Degree => _coefficients.Count - 1;

        public override string ToString()
        {
            return "[" + string.Join(",", _coefficients) + "]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Polynom)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_coefficients.GetHashCode() * 397) ^ Field.GetHashCode();
            }
        }

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
                    throw new ArgumentException("value");

                _coefficients[index] = value;
            }
        }

        /// <summary>
        /// Операция усечения ведущих нулей коэфициентов многочлена
        /// </summary>
        /// <returns>Текущий многочлен без нулевых ведущих коэфициентов</returns>
        public Polynom Truncate()
        {
            int i;
            for (i = Degree; i >= 0 && _coefficients[i] == 0; i--) ;
            _coefficients = _coefficients.Take(i + 1).ToList();

            if (_coefficients.Count == 0)
                _coefficients.Add(0);
            return this;
        }

        /// <summary>
        ///     Увеличивет степень многочлена до заданной, заполняя список коэффициентов нулями
        /// </summary>
        /// <param name="newDegree">Новая степень многочлена</param>
        public Polynom Enlarge(int newDegree)
        {
            if (Degree < newDegree)
                _coefficients.AddRange(Enumerable.Repeat(0, newDegree - Degree));
            return this;
        }

        /// <summary>
        /// Добавляет переданный многочлен к текущему
        /// </summary>
        /// <param name="b">Добавляемый многочлен</param>
        public Polynom Add(Polynom b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

            Enlarge(b.Degree);
            for (var i = 0; i <= b.Degree; i++)
                _coefficients[i] = Field.Add(_coefficients[i], b[i]);
            return Truncate();
        }

        /// <summary>
        /// Вычетает переданный многочлен из текущего
        /// </summary>
        /// <param name="b">Вычетаемый многочлен</param>
        public Polynom Subtract(Polynom b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

            Enlarge(b.Degree);
            for (var i = 0; i <= b.Degree; i++)
                _coefficients[i] = Field.Subtract(_coefficients[i], b[i]);
            return Truncate();
        }

        /// <summary>
        /// Умножает переданный многочлен на число
        /// </summary>
        /// <param name="b">Коэффициент для домножения</param>
        public Polynom Multiply(int b)
        {
            if(Field.IsFieldElement(b) == false)
                throw new ArgumentException("b");

            if (Degree == 0 && _coefficients[0] == 0)
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
        public Polynom Multiply(Polynom b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

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
        /// Вычисляет текущий многочлен по модулю переданного
        /// </summary>
        /// <param name="b">Многочлен, по модулю которого берется текущий</param>
        public Polynom Modulo(Polynom b)
        {
            if (Field.Equals(b.Field) == false)
                throw new ArgumentException("b");

            if (Degree < b.Degree)
                return this;

            for (var i = 0; i <= Degree - b.Degree; i++)
            {
                if(_coefficients[Degree - i] == 0)
                    continue;

                var quotient = Field.Divide(_coefficients[Degree - i], b[b.Degree]);
                for (var j = 0; j <= b.Degree; j++)
                    _coefficients[Degree - i - j] = Field.Add(_coefficients[Degree - i - j],
                        Field.Multiply(b[b.Degree - j], quotient));
            }
            return Truncate();
        }

        /// <summary>
        /// Сдвиг текущего многочлена на указанное количество разрядов вправо
        /// </summary>
        /// <param name="degreeDelta">Количество разрядов, на которые сдвигается многочлен</param>
        public Polynom RightShift(int degreeDelta)
        {
            if (degreeDelta < 0)
                throw new ArgumentException("degreeDelta");
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

        public static Polynom Add(Polynom a, Polynom b)
        {
            var c = new Polynom(a);
            return c.Add(b);
        }

        public static Polynom Subtract(Polynom a, Polynom b)
        {
            var c = new Polynom(a);
            return c.Subtract(b);
        }

        public static Polynom Multiply(Polynom a, int b)
        {
            var c = new Polynom(a);
            return c.Multiply(b);
        }

        public static Polynom Multiply(int a, Polynom b)
        {
            return Multiply(b, a);
        }

        public static Polynom Multiply(Polynom a, Polynom b)
        {
            var c = new Polynom(a);
            return c.Multiply(b);
        }

        public static Polynom Modulo(Polynom a, Polynom b)
        {
            var c = new Polynom(a);
            return c.Modulo(b);
        }

        public static Polynom RightShift(Polynom a, int degreeDelta)
        {
            var c = new Polynom(a);
            return c.RightShift(degreeDelta);
        }

        public static Polynom operator +(Polynom a, Polynom b)
        {
            return Add(a, b);
        }

        public static Polynom operator -(Polynom a, Polynom b)
        {
            return Subtract(a, b);
        }

        public static Polynom operator *(Polynom a, Polynom b)
        {
            return Multiply(a, b);
        }

        public static Polynom operator *(Polynom a, int b)
        {
            return Multiply(a, b);
        }

        public static Polynom operator *(int a, Polynom b)
        {
            return Multiply(a, b);
        }

        public static Polynom operator %(Polynom a, Polynom b)
        {
            return Modulo(a, b);
        }

        public static Polynom operator >>(Polynom a, int degreeDelta)
        {
            return RightShift(a, degreeDelta);
        }
    }
}