namespace AppliedAlgebra.GfAlgorithms.Matrices
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Class represents a matrix whose elements belong to the finite field
    /// </summary>
    public class FieldElementsMatrix
    {
        public class DiagonalizationSummary
        {
            public int PermutationsCount { get; }

            public int?[] SelectedRowsByColumns { get; }

            public FieldElementsMatrix Result { get; }

            public DiagonalizationSummary(int permutationsCount, int?[] selectedRowsByColumns, FieldElementsMatrix result)
            {
                if (selectedRowsByColumns == null)
                    throw new ArgumentNullException(nameof(selectedRowsByColumns));
                if(result == null)
                    throw new ArgumentNullException(nameof(result));

                PermutationsCount = permutationsCount;
                SelectedRowsByColumns = selectedRowsByColumns;
                Result = result;
            }
        }

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
                    throw new ArgumentException("Matrix's elements must not be null");
                if(Field.Equals(element.Field) == false)
                    throw new ArgumentException("Incorrect field");

                _elements[i, j] = new FieldElement(element);
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
            if (i < 0)
                throw new ArgumentNullException($"{nameof(i)} must be not negative");
            if (i >= RowsCount)
                throw new ArgumentException($"{nameof(i)} must be less than matrix rows count");
        }

        /// <summary>
        /// Validates column number correctness
        /// </summary>
        /// <param name="j">Column number</param>
        private void ValidateColumnNumber(int j)
        {
            if (j < 0)
                throw new ArgumentException($"{nameof(j)} must be not negative");
            if (j >= ColumnsCount)
                throw new ArgumentException($"{nameof(j)} must be less than matrix columns count");
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
                    throw new ArgumentException("Incorrect field");

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
                    builder.Append(j == ColumnsCount - 1 ? "\\\\" : " ");
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
        /// Calculates degree <paramref name="degree"/> of the current matrix
        /// </summary>
        /// <param name="degree">Matrix degree</param>
        public FieldElementsMatrix Pow(int degree)
        {
            if (degree < 0)
                throw new ArgumentException($"{nameof(degree)} must not be negative");
            if (RowsCount != ColumnsCount)
                throw new ArgumentException("Matrix must be square");

            var result = IdentityMatrix(Field, RowsCount);
            while (degree > 0)
            {
                if ((degree & 1) == 1)
                    result.Multiply(this);

                Multiply(this);
                degree >>= 1;
            }

            _elements = result._elements;
            return this;
        }

        /// <summary>
        /// Transposes the current matrix
        /// </summary>
        public FieldElementsMatrix Transpose()
        {
            var newElements = new FieldElement[ColumnsCount, RowsCount];
            for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
                newElements[j, i] = _elements[i, j];

            _elements = newElements;
            return this;
        }

        /// <summary>
        /// Swaps elements from rows number <paramref name="firstRowIndex"/> and <paramref name="secondRowsIndex"/> within column <paramref name="columnIndex"/>
        /// </summary>
        private void SwapColumnElements(int firstRowIndex, int secondRowsIndex, int columnIndex)
        {
            var element = _elements[firstRowIndex, columnIndex];
            _elements[firstRowIndex, columnIndex] = _elements[secondRowsIndex, columnIndex];
            _elements[secondRowsIndex, columnIndex] = element;
        }

        /// <summary>
        /// Transforms the current matrix to an diagonal view and returns process summary
        /// <param name="options">Diagonalization process options</param>
        /// </summary>
        public DiagonalizationSummary DiagonalizeExtended(DiagonalizationOptions options = null)
        {
            var permutationsCount = 0;
            var selectedRowsByColumns = new int?[ColumnsCount];
            var opts = options ?? new DiagonalizationOptions();

            var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism};
            for (int col = 0, row = 0; col < ColumnsCount && row < RowsCount; ++col)
            {
                var selectedRow = row;
                for (; selectedRow < RowsCount && _elements[selectedRow, col].Representation == 0; ++selectedRow) ;
                if (selectedRow == RowsCount)
                    continue;

                if (selectedRow != row)
                {
                    Parallel.For(col, ColumnsCount, parallelOptions, j => SwapColumnElements(selectedRow, row, j));
                    ++permutationsCount;
                }

                for (var i = 0; i < RowsCount; ++i)
                    if (i != row)
                    {
                        var c = _elements[i, col] / _elements[row, col];
                        Parallel.For(col, ColumnsCount, parallelOptions, j => _elements[i, j] -= _elements[row, j] * c);
                    }

                selectedRowsByColumns[col] = row;
                ++row;
            }

            return new DiagonalizationSummary(permutationsCount, selectedRowsByColumns, this);
        }

        /// <summary>
        /// Transforms the current matrix to an diagonal view
        /// <param name="options">Diagonalization process options</param>
        /// </summary>
        public FieldElementsMatrix Diagonalize(DiagonalizationOptions options = null) => DiagonalizeExtended(options).Result;


        /// <summary>
        /// Creates column vector whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the vector</param>
        /// <param name="elements">Vector elements</param>
        public static FieldElementsMatrix ColumnVector(GaloisField field, params int[] elements)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            return new FieldElementsMatrix(field, elements.Length, 1, (i, j) => field.CreateElement(elements[i]));
        }

        /// <summary>
        /// Creates column vector
        /// </summary>
        /// <param name="elements">Vector elements</param>
        public static FieldElementsMatrix ColumnVector(params FieldElement[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));
            if (elements.Length == 0)
                throw new ArgumentException($"{nameof(elements)} must have elements");

            var field = elements[0].Field;
            return new FieldElementsMatrix(field, elements.Length, 1, (i, j) => elements[i]);
        }

        /// <summary>
        /// Creates row vector whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the vector</param>
        /// <param name="elements">Vector elements</param>
        public static FieldElementsMatrix RowVector(GaloisField field, params int[] elements)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            return new FieldElementsMatrix(field, 1, elements.Length, (i, j) => field.CreateElement(elements[j]));
        }

        /// <summary>
        /// Creates row vector
        /// </summary>
        /// <param name="elements">Vector elements</param>
        public static FieldElementsMatrix RowVector(params FieldElement[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));
            if (elements.Length == 0)
                throw new ArgumentException($"{nameof(elements)} must have elements");

            var field = elements[0].Field;
            return new FieldElementsMatrix(field, 1, elements.Length, (i, j) => elements[j]);
        }

        /// <summary>
        /// Creates square zero matrix with size <paramref name="size"/>
        /// whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="size">Matrix size</param>
        public static FieldElementsMatrix ZeroMatrix(GaloisField field, int size) => new FieldElementsMatrix(field, size, size);

        /// <summary>
        /// Creates square identity matrix with size <paramref name="size"/>
        /// whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="size">Matrix size</param>
        public static FieldElementsMatrix IdentityMatrix(GaloisField field, int size) =>
            new FieldElementsMatrix(field, size, size, (i, j) => i == j ? field.One() : field.Zero());

        /// <summary>
        /// Creates matrix defined by first row <paramref name="firstRow"/>
        /// cyclically shifted to the right by <paramref name="shift"/>
        /// whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="firstRow">Matrix first row</param>
        /// <param name="shift">First row shift</param>
        public static FieldElementsMatrix FromFirstRow(GaloisField field, int[] firstRow, int? shift = null)
        {
            if(firstRow == null)
                throw new ArgumentNullException(nameof(firstRow));

            var n = firstRow.Length;
            var shiftSize = shift ?? 1;
            if(n % shiftSize != 0)
                throw new ArgumentException("Matrix first row length must be a multiple of the shift size");

            return new FieldElementsMatrix(field, n / shiftSize, n, (i, j) => field.CreateElement(firstRow[(n - i * shiftSize + j) % n]));
        }

        /// <summary>
        /// Creates matrix defined by first row <paramref name="firstRow"/>
        /// cyclically shifted to the right by <paramref name="shift"/>
        /// </summary>
        /// <param name="firstRow">Matrix first row</param>
        /// <param name="shift">First row shift</param>
        public static FieldElementsMatrix FromFirstRow(FieldElement[] firstRow, int? shift = null)
        {
            if (firstRow == null)
                throw new ArgumentNullException(nameof(firstRow));
            if (firstRow.Length == 0)
                throw new ArgumentException($"{nameof(firstRow)} must have elements");

            var n = firstRow.Length;
            var shiftSize = shift ?? 1;
            if (n % shiftSize != 0)
                throw new ArgumentException("Matrix first row length must be a multiple of the shift size");

            var field = firstRow[0].Field;
            return new FieldElementsMatrix(field, n / shiftSize, n, (i, j) => new FieldElement(firstRow[(n - i * shiftSize + j) % n]));
        }

        /// <summary>
        /// Creates circulant matrix defined by first row <paramref name="firstRow"/>
        /// whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="firstRow">Matrix first row</param>
        public static FieldElementsMatrix CirculantMatrix(GaloisField field, params int[] firstRow) =>
            FromFirstRow(field, firstRow);

        /// <summary>
        /// Creates circulant matrix defined by first row <paramref name="firstRow"/>
        /// </summary>
        /// <param name="firstRow">Matrix first row</param>
        public static FieldElementsMatrix CirculantMatrix(params FieldElement[] firstRow) =>
            FromFirstRow(firstRow);

        /// <summary>
        /// Creates double circulant matrix defined by first row <paramref name="firstRow"/>
        /// whose elements must belong to the <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which the elements of the matrix</param>
        /// <param name="firstRow">Matrix first row</param>
        public static FieldElementsMatrix DoubleCirculantMatrix(GaloisField field, params int[] firstRow) =>
            FromFirstRow(field, firstRow, 2);

        /// <summary>
        /// Creates double circulant matrix defined by first row <paramref name="firstRow"/>
        /// </summary>
        /// <param name="firstRow">Matrix first row</param>
        public static FieldElementsMatrix DoubleCirculantMatrix(params FieldElement[] firstRow) =>
            FromFirstRow(firstRow, 2);

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

        /// <summary>
        /// Calculates degree <paramref name="degree"/> of the matrix <paramref name="a"/>
        /// </summary>
        /// <param name="a">Degree basis</param>
        /// <param name="degree">Matrix degree</param>
        public static FieldElementsMatrix Pow(FieldElementsMatrix a, int degree) => new FieldElementsMatrix(a).Pow(degree);

        /// <summary>
        /// Transposes matrix <paramref name="a"/>
        /// </summary>
        /// <param name="a">Transposable matrix</param>
        public static FieldElementsMatrix Transpose(FieldElementsMatrix a) => new FieldElementsMatrix(a).Transpose();

        /// <summary>
        /// Transforms the matrix <paramref name="a"/> to an diagonal view
        /// <param name="a">Diagonalizable matrix</param>
        /// <param name="options">Diagonalization process options</param>
        /// </summary>
        public static FieldElementsMatrix Diagonalize(FieldElementsMatrix a, DiagonalizationOptions options = null) =>
            new FieldElementsMatrix(a).Diagonalize(options);

        /// <summary>
        /// Transforms the matrix <paramref name="a"/> to an diagonal view and returns process summary
        /// <param name="a">Diagonalizable matrix</param>
        /// <param name="options">Diagonalization process options</param>
        /// </summary>
        public static DiagonalizationSummary DiagonalizeExtended(FieldElementsMatrix a, DiagonalizationOptions options = null) =>
            new FieldElementsMatrix(a).DiagonalizeExtended(options);

        public static FieldElementsMatrix operator +(FieldElementsMatrix a, FieldElementsMatrix b) => Add(a, b);

        public static FieldElementsMatrix operator -(FieldElementsMatrix a, FieldElementsMatrix b) => Subtract(a, b);

        public static FieldElementsMatrix operator *(FieldElementsMatrix a, FieldElement b) => Multiply(a, b);

        public static FieldElementsMatrix operator *(FieldElement a, FieldElementsMatrix b) => Multiply(a, b);

        public static FieldElementsMatrix operator *(FieldElementsMatrix a, FieldElementsMatrix b) => Multiply(a, b);
    }
}