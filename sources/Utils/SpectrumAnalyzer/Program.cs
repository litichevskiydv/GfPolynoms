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
        private static IEnumerable<int[]> GenerateCodeWords()
        {
            var field = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });
            var m = new Polynomial(field, new[] {1}).RightShift(7);
            m[0] = 1;
            var f = new Polynomial(field, new[] {0, 4, 2, 0, 3, 2, 6});

            var codeWords = new List<int[]>();
            for (var i = 0; i < field.Order; i++)
                for (var j = 0; j < field.Order; j++)
                    for (var k = 0; k < field.Order; k++)
                    {
                        var informationPolynomial = new Polynomial(field, new[] {k, j, i});
                        var c = (informationPolynomial.RaiseVariableDegre(2)*f)%m;

                        var codeWord = new int[field.Order - 1];
                        for (var l = 0; l <= c.Degree; l++)
                            codeWord[l] = c[l];
                        codeWords.Add(codeWord);
                    }

            return codeWords;
        }

        private static void CheckVector(int[] vector, int[][] codeWords, Dictionary<int, int> spectrum)
        {
            var localResult = Enumerable.Range(0, vector.Length + 1).ToDictionary(x => x, x => 0);
            foreach (var codeWord in codeWords)
            {
                var differencesCount = 0;
                for (var i = 0; i < vector.Length; i++)
                    if (vector[i] != codeWord[i])
                        differencesCount++;
                localResult[differencesCount]++;
            }
            for (var i = 0; i <= vector.Length; i++)
                if (spectrum[i] < localResult[i])
                    spectrum[i] = localResult[i];
        }

        private static void CheckVectors(int[] vector, int position, int[][] codeWords, Dictionary<int, int> spectrum)
        {
            if (position == vector.Length)
            {
                CheckVector(vector, codeWords, spectrum);
                return;
            }

            for (var  i = 0; i < 8; i++)
            {
                vector[position] = i;
                CheckVectors(vector, position + 1, codeWords, spectrum);
            }
        }

        static void Main()
        {
            Console.WriteLine("Start code words generation");
            var generationTimer = Stopwatch.StartNew();
            var codeWords = GenerateCodeWords().ToArray();
            generationTimer.Stop();
            Console.WriteLine("Code words generated in {0} seconds", generationTimer.Elapsed.TotalSeconds);

            Console.WriteLine("Start scectrum calculation");
            var calculationTimer = Stopwatch.StartNew();
            var masks = Enumerable.Range(0, 8)
                .Select(x => Enumerable.Repeat(x, 7).ToArray())
                .ToArray();
            var results = Enumerable.Range(0, 8)
                .ToDictionary(x => x, x => Enumerable.Range(0, 8).ToDictionary(y => y, y => 0));
            Parallel.ForEach(masks, new ParallelOptions {MaxDegreeOfParallelism = 12},
                x => CheckVectors(x, 1, codeWords, results[x[0]]));
            var result = results
                .SelectMany(x => x.Value)
                .GroupBy(x => x.Key)
                .Select(x => new Tuple<int, int>(x.Key, x.Max(y => y.Value)))
                .ToArray();
            calculationTimer.Stop();
            Console.WriteLine("Scectrum calculated in {0} seconds", calculationTimer.Elapsed.TotalSeconds);

            foreach (var pair in result)
                Console.WriteLine("Errors count {0} code words {1}", pair.Item1, pair.Item2);
            Console.ReadKey();
        }
    }
}
