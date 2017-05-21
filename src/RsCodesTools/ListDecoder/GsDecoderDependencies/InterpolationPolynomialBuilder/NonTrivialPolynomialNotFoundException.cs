namespace AppliedAlgebra.RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;

    /// <summary>
    /// Exception for indicating only existence of zero interpolation polynomial 
    /// </summary>
    public class NonTrivialPolynomialNotFoundException : InvalidOperationException
    {
    }
}