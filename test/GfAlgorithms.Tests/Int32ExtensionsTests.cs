namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using Extensions;
    using Xunit;

    public class Int32ExtensionsTests
    {
        [Fact]
        public void PowMustThrowExceptionForNegativeDegrees()
        {
            Assert.Throws<ArgumentException>(() => 5.Pow(-1));
        }
    }
}