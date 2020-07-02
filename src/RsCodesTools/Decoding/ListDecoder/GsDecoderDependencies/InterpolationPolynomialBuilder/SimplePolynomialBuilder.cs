namespace AppliedAlgebra.RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.BiVariablePolynomials;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Implementation of interpolation polynomial builder based on linear systems solving
    /// </summary>
    public class SimplePolynomialBuilder : IInterpolationPolynomialBuilder
    {
        /// <summary>
        /// Implementation of combinations count calculator contract
        /// </summary>
        private readonly ICombinationsCountCalculator _combinationsCountCalculator;
        /// <summary>
        /// Implementation of linear systems solver contract
        /// </summary>
        private readonly ILinearSystemSolver _linearSystemSolver;

        /// <summary>
        /// Method for numbering bivariate monomials
        /// </summary>
        /// <param name="variableIndexByMonomial">Mapping for bivariate monomials to their numbers</param>
        /// <param name="monomialByVariableIndex">Mapping from numbers to bivariate monomials</param>
        /// <param name="xDegree">Degree of bivariate monomial's x variable</param>
        /// <param name="yDegree">Degree of bivariate monomial's y variable</param>
        /// <returns>Monomial's number</returns>
        private static int GetVariableIndexByMonomial(
            IDictionary<(int xDegree, int yDegree), int> variableIndexByMonomial,
            IDictionary<int, (int xDegree, int yDegree)> monomialByVariableIndex,
            int xDegree, 
            int yDegree)
        {
            var monomial = (xDegree, yDegree);
            int variableIndex;

            if (variableIndexByMonomial.TryGetValue(monomial, out variableIndex) == false)
            {
                variableIndex = variableIndexByMonomial.Count;
                variableIndexByMonomial[monomial] = variableIndex;
                monomialByVariableIndex[variableIndex] = monomial;
            }
            return variableIndex;
        }

        /// <summary>
        /// Method for creating linear equations system for finding interpolation polynomial
        /// </summary>
        /// <param name="field">Finite field from which system coefficients'll be taken</param>
        /// <param name="degreeWeight">Weight of bivariate monomials degree</param>
        /// <param name="maxWeightedDegree">Maximum value of bivariate monomial degree</param>
        /// <param name="maxXDegree">Maximum value of bivariate monomial x variable degree</param>
        /// <param name="roots">Roots of the interpolation polynomial</param>
        /// <param name="rootsMultiplicity">Multiplicity of bivariate polynomial's roots</param>
        /// <param name="variableIndexByMonomial">Mapping for bivariate monomials to their numbers</param>
        /// <param name="monomialByVariableIndex">Mapping from numbers to bivariate monomials</param>
        /// <returns>Linear system's representation</returns>
        private List<List<Tuple<int, FieldElement>>> BuildEquationsSystem(
            GaloisField field,
            (int xWeight, int yWeight) degreeWeight,
            int maxWeightedDegree, 
            int maxXDegree, 
            IEnumerable<Tuple<FieldElement, FieldElement>> roots, 
            int rootsMultiplicity,
            IDictionary<(int xDegree, int yDegree), int> variableIndexByMonomial, 
            IDictionary<int, (int xDegree, int yDegree)> monomialByVariableIndex)
        {
            var equations = new List<List<Tuple<int, FieldElement>>>();
            var combinationsCache = new FieldElement[Math.Max(maxWeightedDegree/degreeWeight.xWeight, maxWeightedDegree/degreeWeight.yWeight) + 1][]
                    .MakeSquare();

            foreach (var root in roots)
                for (var r = 0; r < rootsMultiplicity; r++)
                    for (var s = 0; r + s < rootsMultiplicity; s++)
                    {
                        var equation = new List<Tuple<int, FieldElement>>();
                        equations.Add(equation);

                        for (var i = r; i <= maxXDegree; i++)
                            for (var j = s; i*degreeWeight.xWeight + j*degreeWeight.yWeight <= maxWeightedDegree; j++)
                            {
                                var variableIndex = GetVariableIndexByMonomial(variableIndexByMonomial, monomialByVariableIndex, i, j);
                                var coefficient = _combinationsCountCalculator.Calculate(field, i, r, combinationsCache)
                                                  *_combinationsCountCalculator.Calculate(field, j, s, combinationsCache)
                                                  *FieldElement.Pow(root.Item1, i - r)
                                                  *FieldElement.Pow(root.Item2, j - s);

                                equation.Add(new Tuple<int, FieldElement>(variableIndex, coefficient));
                            }
                    }

            return equations;
        }

        /// <summary>
        /// Method for transforming linear system matrix into rectangular form and finding system solution
        /// </summary>
        /// <param name="field">Finite field from which system coefficients were taken</param>
        /// <param name="variablesCount">Variables count</param>
        /// <param name="equationsSystem">Linear system's representation</param>
        /// <returns>Linear system's solution</returns>
        private SystemSolution SolveEquationsSystem(
            GaloisField field,
            int variablesCount, 
            IReadOnlyList<List<Tuple<int, FieldElement>>> equationsSystem)
        {
            var linearSystemMatrix = new FieldElement[equationsSystem.Count, variablesCount];
            var zeros = new FieldElement[equationsSystem.Count];
            for (var i = 0; i < equationsSystem.Count; i++)
            {
                zeros[i] = field.Zero();
                for (var j = 0; j < variablesCount; j++)
                    linearSystemMatrix[i, j] = field.Zero();
            }
            for (var i = 0; i < equationsSystem.Count; i++)
                for (var j = 0; j < equationsSystem[i].Count; j++)
                    linearSystemMatrix[i, equationsSystem[i][j].Item1] = equationsSystem[i][j].Item2;

            return _linearSystemSolver.Solve(linearSystemMatrix, zeros);
        }

        /// <summary>
        /// Method for creation interpolation polynomial from linear system solution
        /// </summary>
        /// <param name="field">Finite field from which system coefficients were taken</param>
        /// <param name="systemSolution">Solution of the linear equations system</param>
        /// <param name="monomialByVariableIndex">Mapping from numbers to bivariate monomials</param>
        /// <returns>Interpolation polynomial</returns>
        private static BiVariablePolynomial ConstructInterpolationPolynomial(
            GaloisField field,
            SystemSolution systemSolution, 
            IReadOnlyDictionary<int, (int xDegree, int yDegree)> monomialByVariableIndex)
        {
            var interpolationPolynomial = new BiVariablePolynomial(field);
            for (var i = 0; i < systemSolution.VariablesValues.Length; i++)
                interpolationPolynomial[monomialByVariableIndex[i]] = systemSolution.VariablesValues[i];

            if (interpolationPolynomial.IsZero)
                throw new NonTrivialPolynomialNotFoundException();
            return interpolationPolynomial;
        }

        /// <summary>
        /// Method for bivariate interpolation polynomial building
        /// </summary>
        /// <param name="degreeWeight">Weight of bivariate monomials degree</param>
        /// <param name="maxWeightedDegree">Maximum value of bivariate monomial degree</param>
        /// <param name="roots">Roots of the interpolation polynomial</param>
        /// <param name="rootsMultiplicity">Multiplicity of bivariate polynomial's roots</param>
        /// <returns>Builded interpolation polynomial</returns>
        public BiVariablePolynomial Build(
            (int xWeight, int yWeight) degreeWeight, 
            int maxWeightedDegree, 
            Tuple<FieldElement, FieldElement>[] roots, 
            int rootsMultiplicity)
        {
            if (roots == null)
                throw new ArgumentNullException(nameof(roots));
            if (roots.Length == 0)
                throw new ArgumentException($"{nameof(roots)} is empty");

            var field = roots[0].Item1.Field;
            var variableIndexByMonomial = new Dictionary<(int xDegree, int yDegree), int>();
            var monomialByVariableIndex = new Dictionary<int, (int xDegree, int yDegree)>();

            var equationsSystem = BuildEquationsSystem(field, degreeWeight,
                maxWeightedDegree, maxWeightedDegree / degreeWeight.xWeight, roots, rootsMultiplicity,
                variableIndexByMonomial, monomialByVariableIndex);

            var systemSolution = SolveEquationsSystem(field, variableIndexByMonomial.Count, equationsSystem);

            return ConstructInterpolationPolynomial(field, systemSolution, monomialByVariableIndex);
        }

        /// <summary>
        /// Constructor for creating instance of interpolation polynomial builder
        /// </summary>
        /// <param name="combinationsCountCalculator">Implementation of combinations count calculator contract</param>
        /// <param name="linearSystemSolver">Implementation of linear systems solver contract</param>
        public SimplePolynomialBuilder(ICombinationsCountCalculator combinationsCountCalculator, ILinearSystemSolver linearSystemSolver)
        {
            if (combinationsCountCalculator == null)
                throw new ArgumentNullException(nameof(combinationsCountCalculator));
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _combinationsCountCalculator = combinationsCountCalculator;
            _linearSystemSolver = linearSystemSolver;
        }
    }
}