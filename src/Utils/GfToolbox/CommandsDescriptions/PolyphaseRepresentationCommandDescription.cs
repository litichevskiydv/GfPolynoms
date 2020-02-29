namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System.CommandLine;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [UsedImplicitly]
    public class PolyphaseRepresentationCommandDescription : CommandDescriptionBase
    {
        private readonly ILogger _logger;

        public PolyphaseRepresentationCommandDescription(IConfiguration configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
            _logger = LoggerFactory.CreateLogger<PolyphaseRepresentationCommandDescription>();

            CommandLineOptions.Add(new Option<int>("--max-degree", "Maximum possible degree of the complementary polynomials") {Required = true});
        }

        public override string Name => "polyphase-representation";
        public override string Description => "Computes polyphase representation for the given polynomial";

        [UsedImplicitly]
        public void Execute(int fieldOrder, int[] fieldIrreduciblePolynomial, int maxDegree)
        {
            _logger.LogInformation($"Field order: {fieldOrder}");
            var coeffs = fieldIrreduciblePolynomial != null
                ? string.Join(", ", fieldIrreduciblePolynomial)
                : "";
            _logger.LogInformation($"Field irreducible polynomial coefficients: {coeffs}");
            _logger.LogInformation($"Max degree: {maxDegree}");
        }
    }
}