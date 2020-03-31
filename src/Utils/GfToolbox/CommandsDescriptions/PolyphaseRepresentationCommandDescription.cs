namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System.CommandLine;
    using System.Linq;
    using GfAlgorithms.ComplementaryRepresentationFinder;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [UsedImplicitly]
    public class PolyphaseRepresentationCommandDescription : CommandDescriptionBase
    {
        private readonly IComplementaryRepresentationFinder _linearSystemsBasedFinder;
        private readonly IComplementaryRepresentationFinder _bruteForceBasedFinder;
        private readonly ILogger _logger;

        public PolyphaseRepresentationCommandDescription(IConfiguration configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
            _linearSystemsBasedFinder = new LinearEquationsBasedFinder();
            _bruteForceBasedFinder = new BruteForceBasedFinder(new RecursiveIterator());
            _logger = LoggerFactory.CreateLogger<PolyphaseRepresentationCommandDescription>();

            Options.Add(
                new Option<int[]>("--polynomial-coefficients")
                {
                    Required = true,
                    Argument = new Argument<int[]>(result => result.Tokens.Select(x => int.Parse(x.Value.Trim(','))).ToArray())
                               {
                                   Arity = ArgumentArity.OneOrMore
                               },
                    Description = "Coefficients of the polynomial whose polyphase representation must be computed"
                }
            );
            Options.Add(new Option<int>("--max-degree", "Maximum possible degree of the polynomials in the representation") {Required = true});
            Options.Add(new Option<int?>("--lambda", "Lambda parameter value in the representation"));
        }

        protected override string Name => "polyphase-representation";
        protected override string Description => "Computes polyphase representation for the given polynomial";

        private IComplementaryRepresentationFinder ChoseImplementation(Polynomial polynomial, int maxDegree)
        {
            var coefficientsCount = maxDegree + 1;
            if (coefficientsCount % 2 == 1)
                return _bruteForceBasedFinder;

            return polynomial.Field.Characteristic == 2 ? _bruteForceBasedFinder : _linearSystemsBasedFinder;
        }

        [UsedImplicitly]
        public void Execute(GaloisField field, int[] polynomialCoefficients, int maxDegree, int? lambda)
        {
            var polynomial = new Polynomial(field, polynomialCoefficients);
            var lambdaValue = lambda.HasValue ? field.CreateElement(lambda.Value) : field.One();

            var (h, g) = ChoseImplementation(polynomial, maxDegree)
                .Find(polynomial, maxDegree, lambdaValue)
                .First();
            _logger.LogInformation("Polyphase representation: {f}={h}+{lambda}*x^2({g})", polynomial, h, lambdaValue, g);
        }
    }
}