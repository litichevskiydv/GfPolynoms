namespace WaveletCodesTools.GeneratingPolynomialsBuilder
{
    using GfPolynoms;

    public interface IGeneratingPolynomialsBuilder
    {
        Polynomial Build(int n, int d, Polynomial sourceFilter);
    }
}