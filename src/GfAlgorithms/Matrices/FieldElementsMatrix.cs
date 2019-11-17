namespace AppliedAlgebra.GfAlgorithms.Matrices
{
    using System;
    using System.Text;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Class represents a matrix whose elements belong to the finite field
    /// </summary>
    public class FieldElementsMatrix
    {
        /// <summary>
        /// Field from which the elements of the matrix
        /// </summary>
        public GaloisField Field { get; }

        /// <summary>
        /// Matrix elements
        /// </summary>
        private FieldElement[,] _elements;

        /// <summary>
        /// Initializes matrix elements
        /// </summary>
        /// <param name="elementInitializer">Element initializer</param>
        private void InitializeElements(Func<int, int, FieldElement> elementInitializer)
        {
            for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
            {
                var element = elementInitializer(i, j);
                if(element == null)
                    throw new InvalidOperationException("Matrix's elements must not be null");
                if(Field.Equals(element.Field) == false)
                    throw new InvalidOperationException("Incorrect field");

                _elements[i, j] = element;
            }
        }

        /// <summary>
        /// Creates matrix <paramref name="rowsCount"/>*<paramref name="columnsCount"/>
        /// whose elements must belong to the <paramref name="field"/> and initialized
        /// via <paramref name="elementInitializer"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="rowsCount">New matrix rows count</param>
        /// <param name="columnsCount">New matrix columns count</param>
        /// <param name="elementInitializer">Element initializer</param>
        public FieldElementsMatrix(GaloisField field, int rowsCount, int columnsCount, Func<int, int, FieldElement> elementInitializer = null)
        {
            if (field == null)
                throw new ArgumentException(nameof(field));
            if (rowsCount <= 0)
                throw new ArgumentException($"{nameof(rowsCount)} must be positive");
            if (columnsCount <= 0)
                throw new ArgumentException($"{nameof(columnsCount)} must be positive");

            Field = field;

            _elements = new FieldElement[rowsCount, columnsCount];
            InitializeElements(elementInitializer ?? ((i, j) => field.Zero()));
        }

        /// <summary>
        /// Creates matrix with elements taken from <paramref name="elements"/>
        /// over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="elements">Matrix elements</param>
        public FieldElementsMatrix(GaloisField field, int[,] elements)
        {
            if (field == null)
                throw new ArgumentException(nameof(field));
            if(elements == null)
                throw new ArgumentNullException(nameof(elements));
            if(elements.GetLength(0) == 0)
                throw new ArgumentException($"{nameof(elements)} must have nonzero rows count");
            if (elements.GetLength(1) == 0)
                throw new ArgumentException($"{nameof(elements)} must have nonzero columns count");

            Field = field;

            _elements = new FieldElement[elements.GetLength(0), elements.GetLength(1)];
            InitializeElements((i, j) => field.CreateElement(elements[i, j]));
        }

        /// <summary>
        /// Creates matrix with elements taken from <paramref name="elements"/>
        /// </summary>
        /// <param name="elements">Matrix elements</param>
        public FieldElementsMatrix(FieldElement[,] elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));
            if (elements.GetLength(0) == 0)
                throw new ArgumentException($"{nameof(elements)} must have nonzero rows count");
            if (elements.GetLength(1) == 0)
                throw new ArgumentException($"{nameof(elements)} must have nonzero columns count");
            if(elements[0, 0] == null)
                throw new ArgumentException("Matrix's elements must not be null");

            Field = elements[0, 0].Field;

            _elements = new FieldElement[elements.GetLength(0), elements.GetLength(1)];
            InitializeElements((i, j) => elements[i, j]);
        }

        /// <summary>
        /// Creates a copy of the given matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">The copied matrix</param>
        public FieldElementsMatrix(FieldElementsMatrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            Field = matrix.Field;

            _elements = new FieldElement[matrix.RowsCount, matrix.ColumnsCount];
            InitializeElements((i, j) => matrix._elements[i, j]);
        }

        /// <summary>
        /// Validates row number correctness
        /// </summary>
        /// <param name="i">Row number</param>
        private void ValidateRowNumber(int i)
        {
            if (i <= 0)
                throw new ArgumentNullException($"{nameof(i)} must be positive");
            if (i > RowsCount)
                throw new ArgumentException($"{nameof(i)} mustn't be greater than matrix rows count");
        }

        /// <summary>
        /// Validates column number correctness
        /// </summary>
        /// <param name="j">Column number</param>
        private void ValidateColumnNumber(int j)
        {
            if (j <= 0)
                throw new ArgumentException($"{nameof(j)} must be positive");
            if (j > ColumnsCount)
                throw new ArgumentException($"{nameof(j)} mustn't be greater than matrix columns count");
        }

        /// <summary>
        /// Matrix rows count
        /// </summary>
        public int RowsCount => _elements.GetLength(0);
        /// <summary>
        /// Matrix columns count
        /// </summary>
        public int ColumnsCount => _elements.GetLength(1);
        /// <summary>
        /// Indexer for obtaining matrix element by row number <paramref name="i"/> and column number <paramref name="j"/>
        /// </summary>
        /// <param name="i">Row number</param>
        /// <param name="j">Column number</param>
        /// <returns>Matrix element</returns>
        public FieldElement this[int i, int j]
        {
            get
            {
                ValidateRowNumber(i);
                ValidateColumnNumber(j);


                return _elements[i, j];
            }
            set
            {
                ValidateRowNumber(i);
                ValidateColumnNumber(j);

                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (Field.Equals(value.Field) == false)
                    throw new AggregateException("Incorrect field");

                _elements[i, j] = value;
            }
        }

        /// <summary>
        /// Checks the equality of the current matrix to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another matrix</param>
        /// <returns>Checking result</returns>
        protected bool Equals(FieldElementsMatrix other)
        {
            if (Field.Equals(other.Field) == false)
                return false;
            if (RowsCount != other.RowsCount || ColumnsCount != other.ColumnsCount)
                return false;

            for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
                if (_elements[i, j].Equals(other._elements[i, j]) == false)
                    return false;

            return true;
        }

        /// <summary>
        /// Checks the equality of the current matrix to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Another matrix</param>
        /// <returns>Checking result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FieldElementsMatrix)obj);
        }

        /// <summary>
        /// Calculates matrix hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            var hashCode = Field.GetHashCode();

            unchecked
            {
                for (var i = 0; i < RowsCount; i++)
                for (var j = 0; j < ColumnsCount; j++)
                    hashCode = hashCode * 31 ^ _elements[i, j].GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Method for obtaining a string representation of the current matrix
        /// </summary>
        /// <returns>String representation of the current matrix</returns>
        public override string ToString()
        {
            var builder = new StringBuilder()
                .AppendLine(@"\begin{bmatrix}");

            for (var i = 0; i < RowsCount; i++)
            {
                builder.Append("\t");

                for (var j = 0; j < ColumnsCount; j++)
                {
                    if (j != 0) builder.Append("&");
                    builder.Append(_elements[i, j]);
                    builder.Append(j == ColumnsCount - 1 ? "\\" : " ");
                }

                builder.AppendLine();
            }

            return builder.AppendLine(@"\end{bmatrix}").ToString();
        }

        /// <summary>
        /// Validates matrix <paramref name="argument"/> for addition or subtraction
        /// </summary>
        /// <param name="argument">Matrix for validation</param>
        private void ValidateAdditionArgument(FieldElementsMatrix argument)
        {
            if(argument == null)
                throw new ArgumentNullException(nameof(argument));
            if (Field.Equals(argument.Field) == false)
                throw new ArgumentException("Matrices fields must be the same");
            if (RowsCount != argument.RowsCount)
                throw new ArgumentException("Matrices rows count must be the same");
            if (ColumnsCount != argument.ColumnsCount)
                throw new ArgumentException("Matrices columns count must be the same");
        }

        /// <summary>
        /// Adds matrix <paramref name="other"/> to current
        /// </summary>
        /// <param name="other">Term</param>
        /// <returns>Modified matrix</returns>
        public FieldElementsMatrix Add(FieldElementsMatrix other)
        {
            ValidateAdditionArgument(other);

            for (var i =0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
                _elements[i, j].Add(other._elements[i, j]);

            return this;
        }

        /// <summary>
        /// Subtracts <paramref name="other"/> from current
        /// </summary>
        /// <param name="other">Subtrahend</param>
        /// <returns>Modified matrix</returns>
        public FieldElementsMatrix Subtract(FieldElementsMatrix other)
        {
            ValidateAdditionArgument(other);

            for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
                _elements[i, j].Subtract(other._elements[i, j]);

            return this;
        }

        /// <summary>
        /// Multiplies current matrix by element <paramref name="element"/>
        /// </summary>
        /// <param name="element">Multiplier</param>
        /// <returns>Modified matrix</returns>
        public FieldElementsMatrix Multiply(FieldElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if(Field.Equals(element.Field) == false)
                throw new ArgumentException("Fields of the matrix and element  must be the same");

            for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
                _elements[i, j].Multiply(element);

            return this;
        }

        /// <summary>
        /// Multiplies current matrix by matrix <paramref name="other"/>
        /// </summary>
        /// <param name="other">Multiplier</param>
        /// <returns>Modified matrix</returns>
        public FieldElementsMatrix Multiply(FieldElementsMatrix other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (Field.Equals(other.Field) == false)
                throw new ArgumentException("Matrices fields must be the same");
            if (ColumnsCount != other.RowsCount)
                throw new ArgumentException("Matrices dimensions must be consistent");

            var newElements = new FieldElement[RowsCount, other.ColumnsCount];
            for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < other.ColumnsCount; j++)
            {
                newElements[i, j] = Field.Zero();
                for (var k = 0; k < ColumnsCount; k++)
                    newElements[i, j].Add(_elements[i, k] * other._elements[k, j]);
            }

            _elements = newElements;
            return this;
        }

        /// <summary>
        /// Adds matrix <paramref name="b"/> to <paramref name="a"/>
        /// </summary>
        /// <param name="a">First term</param>
        /// <param name="b">Second term</param>
        public static FieldElementsMatrix Add(FieldElementsMatrix a, FieldElementsMatrix b) => new FieldElementsMatrix(a).Add(b);

        /// <summary>
        /// Subtracts matrix <paramref name="a"/> to <paramref name="a"/>
        /// </summary>
        /// <param name="a">Minuend</param>
        /// <param name="b">Subtrahend</param>
        public static FieldElementsMatrix Subtract(FieldElementsMatrix a, FieldElementsMatrix b) => new FieldElementsMatrix(a).Subtract(b);

        /// <summary>
        /// Multiplies matrix <paramref name="a"/> by element <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static FieldElementsMatrix Multiply(FieldElementsMatrix a, FieldElement b) => new FieldElementsMatrix(a).Multiply(b);

        /// <summary>
        /// Multiplies matrix <paramref name="b"/> by element <paramref name="a"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static FieldElementsMatrix Multiply(FieldElement a, FieldElementsMatrix b) => new FieldElementsMatrix(b).Multiply(a);

        /// <summary>
        /// Multiplies matrix <paramref name="a"/> by matrix <paramref name="b"/>
        /// </summary>
        /// <param name="a">First factor</param>
        /// <param name="b">Second factor</param>
        public static FieldElementsMatrix Multiply(FieldElementsMatrix a, FieldElementsMatrix b) => new FieldElementsMatrix(a).Multiply(b);

        public static FieldElementsMatrix operator +(FieldElementsMatrix a, FieldElementsMatrix b) => Add(a, b);

        public static FieldElementsMatrix operator -(FieldElementsMatrix a, FieldElementsMatrix b) => Subtract(a, b);

        public static FieldElementsMatrix operator *(FieldElementsMatrix a, FieldElement b) => Multiply(a, b);

        public static FieldElementsMatrix operator *(FieldElement a, FieldElementsMatrix b) => Multiply(a, b);

        public static FieldElementsMatrix operator *(FieldElementsMatrix a, FieldElementsMatrix b) => Multiply(a, b);
    }
}