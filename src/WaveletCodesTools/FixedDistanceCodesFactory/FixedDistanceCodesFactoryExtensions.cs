namespace AppliedAlgebra.WaveletCodesTools.FixedDistanceCodesFactory
{
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public static class FixedDistanceCodesFactoryExtensions
    {
        public static ICode CreateN7K3D4(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(
                    new PrimePowerOrderField(8, new Polynomial(GaloisField.Create(2), 1, 1, 0, 1)),
                    3, 2, 7, 6, 4, 2
                ),
                4
            );

        public static ICode CreateN8K4D4(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(new PrimePowerOrderField(9), 2, 0, 1, 7, 2, 1), 4);

        public static ICode CreateN10K5D5(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(GaloisField.Create(11), 7, 1, 9, 0, 5, 4, 10, 2, 2, 1), 5);

        public static ICode CreateN15K7D8(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(
                    new PrimePowerOrderField(16, new Polynomial(GaloisField.Create(2), 1, 0, 0, 1, 1)),
                    3, 2, 7, 6, 4, 2, 11, 7, 5
                ),
                8
            );

        public static ICode CreateN26K13D12(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(
                    new PrimePowerOrderField(27, new Polynomial(GaloisField.Create(3), 2, 2, 0, 1)),
                    22, 0, 0, 1, 2, 3, 4, 1, 6, 7, 8, 9, 1, 10, 1, 12, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1
                ),
                12
            );

        public static ICode CreateN30K15D13(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(GaloisField.Create(31), 22, 0, 2, 1, 27, 8, 4, 18, 6, 9, 8, 17, 18), 13);

        public static ICode CreateN31K15D15(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(
                    new PrimePowerOrderField(32, new Polynomial(GaloisField.Create(2), 1, 0, 0, 1, 0, 1)),
                    23, 13, 27, 1, 15, 13, 1, 16, 1, 21, 28, 30, 12, 19, 17, 4, 1, 19, 14, 0, 3, 5, 6
                ),
                15
            );

        public static ICode CreateN80K40D37(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(
                    new PrimePowerOrderField(81, new Polynomial(GaloisField.Create(3), 2, 0, 0, 2, 1)),
                    0, 0, 0, 50, 2, 3, 45, 1, 6, 7, 8, 9, 19, 10, 1, 80, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1, 55, 77, 42, 11
                ),
                37
            );

        public static ICode CreateN100K50D49(this IFixedDistanceCodesFactory factory) =>
            factory.Create(new Polynomial(GaloisField.Create(101), 78, 2, 67, 50, 2, 45, 45, 20, 77, 7, 42, 56, 0, 67, 60, 81), 49);
    }
}