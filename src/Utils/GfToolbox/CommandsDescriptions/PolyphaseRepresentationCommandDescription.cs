namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System.CommandLine;
    using GfPolynoms.GaloisFields;
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

            Options.Add(new Option<int>("--max-degree", "Maximum possible degree of the complementary polynomials") {Required = true});
        }

        protected override string Name => "polyphase-representation";
        protected override string Description => "Computes polyphase representation for the given polynomial";

        [UsedImplicitly]
        public void Execute(GaloisField field, int maxDegree)
        {
            _logger.LogInformation($"Field: {field}");
            _logger.LogInformation($"Max degree: {maxDegree}");
        }
    }
}