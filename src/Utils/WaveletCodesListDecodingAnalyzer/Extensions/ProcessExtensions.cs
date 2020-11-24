namespace AppliedAlgebra.WaveletCodesListDecodingAnalyzer.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public static class ProcessExtensions
    {
        public static int ConstrainProcessorUsage(this Process process, int physicalProcessorsCount, double processorLoadingPercent)
        {
            var processorCoresCount = Environment.ProcessorCount / physicalProcessorsCount;
            var processorActiveCoresCount = (int) (processorLoadingPercent * processorCoresCount);

            var affinityMask = (IntPtr) Convert.ToUInt64(
                string.Join(
                    string.Empty,
                    Enumerable.Repeat(
                        string.Join(
                            string.Empty,
                            Enumerable.Repeat("1", processorActiveCoresCount)
                                .Concat(
                                    Enumerable.Repeat("0", processorCoresCount - processorActiveCoresCount)
                                )
                        ),
                        physicalProcessorsCount
                    )
                ),
                2
            );
            process.ProcessorAffinity = affinityMask;

            return physicalProcessorsCount * processorActiveCoresCount;
        }
    }
}