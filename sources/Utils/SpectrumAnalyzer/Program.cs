namespace SpectrumAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GfPolynoms;
    using GfPolynoms.GaluaFields;

    static class Program
    {
        private static void CalculateCodeWords(int fieldOrder, int[] informationWord, int informationWordPosition, Func<int[], int[]> codeWordCalculator, ICollection<int[]> codeWords)
        {
            if (informationWordPosition == informationWord.Length)
                codeWords.Add(codeWordCalculator(informationWord).ToArray());
            else
                for (var i = 0; i < fieldOrder; i++)
                {
                    informationWord[informationWordPosition] = i;
                    CalculateCodeWords(fieldOrder, informationWord, informationWordPosition + 1, codeWordCalculator, codeWords);
                }
        }

        private static IEnumerable<int[]> GenerateCodeWords(int fieldOrder, int informationWordLength, Func<int[], int[]> codeWordCalculator)
        {
            var informationWord = new int[informationWordLength];
            var codeWords = new List<int[]>();

            CalculateCodeWords(fieldOrder, informationWord, 0, codeWordCalculator, codeWords);
            return codeWords;
        }

        private static void UpdateTaskSpectrum(IReadOnlyList<int> vector, IEnumerable<int[]> codeWords, IDictionary<int, int> spectrum)
        {
            var localResult = Enumerable.Range(0, vector.Count + 1).ToDictionary(x => x, x => 0);
            foreach (var codeWord in codeWords)
            {
                var differencesCount = 0;
                for (var i = 0; i < vector.Count; i++)
                    if (vector[i] != codeWord[i])
                        differencesCount++;
                localResult[differencesCount]++;
            }
            for (var i = 0; i <= vector.Count; i++)
                if (spectrum[i] < localResult[i])
                    spectrum[i] = localResult[i];
        }

        private static void CheckVectors(int fieldOrder, int[] vector, int position, int[][] codeWords, IDictionary<int, int> spectrum)
        {
            if (position == vector.Length)
                UpdateTaskSpectrum(vector, codeWords, spectrum);
            else
                for (var i = 0; i < fieldOrder; i++)
                {
                    vector[position] = i;
                    CheckVectors(fieldOrder, vector, position + 1, codeWords, spectrum);
                }
        }

        private static void CalculateSpectrum(int fieldOrder, int informationWordLength, int codeWordLength, Func<int[], int[]> codeWordCalculator)
        {
            Console.WriteLine("Start code words generation");
            var generationTimer = Stopwatch.StartNew();
            var codeWords = GenerateCodeWords(fieldOrder, informationWordLength, codeWordCalculator).ToArray();
            generationTimer.Stop();
            Console.WriteLine("Code words generated in {0} seconds", generationTimer.Elapsed.TotalSeconds);

            Console.WriteLine("Start scectrum calculation");
            var calculationTimer = Stopwatch.StartNew();
            var masks = Enumerable.Range(0, fieldOrder)
                .Select(x => Enumerable.Repeat(x, codeWordLength).ToArray())
                .ToArray();
            var results = Enumerable.Range(0, fieldOrder)
                .ToDictionary(x => x, x => Enumerable.Range(0, codeWordLength + 1).ToDictionary(y => y, y => 0));
            Parallel.ForEach(masks, new ParallelOptions { MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 1.5d) },
                x => CheckVectors(fieldOrder, x, 1, codeWords, results[x[0]]));
            var result = results
                .SelectMany(x => x.Value)
                .GroupBy(x => x.Key)
                .Select(x => new Tuple<int, int>(x.Key, x.Max(y => y.Value)))
                .ToArray();
            calculationTimer.Stop();
            Console.WriteLine("Scectrum calculated in {0} seconds", calculationTimer.Elapsed.TotalSeconds);

            foreach (var pair in result)
                Console.WriteLine("Errors count {0} code words {1}", pair.Item1, pair.Item2);
        }

        private static void CalculateSpectrumForWavelet37Code()
        {
            const int informationWordLength = 3;
            const int codeWordLength = 7;

            var field = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });
            var m = new Polynomial(field, 1).RightShift(7);
            m[0] = 1;
            var f = new Polynomial(field, 0, 4, 2, 0, 3, 2, 6);

            CalculateSpectrum(field.Order, informationWordLength, codeWordLength,
                informatonWord =>
                {
                    var informationPolynomial = new Polynomial(field, informatonWord);
                    var c = (informationPolynomial.RaiseVariableDegre(2)*f)%m;

                    var codeWord = new int[field.Order - 1];
                    for (var i = 0; i <= c.Degree; i++)
                        codeWord[i] = c[i];
                    return codeWord;
                });
        }

        static void Main()
        {
            CalculateSpectrumForWavelet37Code();
            Console.ReadKey();
        }
    }
}
