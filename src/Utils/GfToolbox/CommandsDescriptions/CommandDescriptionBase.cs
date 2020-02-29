namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public abstract class CommandDescriptionBase : ICommandDescription
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly List<Option> Options;

        protected abstract string Name { get; }
        protected abstract string Description { get; }

        protected CommandDescriptionBase(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            Configuration = configuration;
            LoggerFactory = loggerFactory;

            Options
                = new List<Option>
                  {
                      new Option<int>("--field-order", "Finite field order") {Required = true},
                      new Option<int[]>("--field-irreducible-polynomial", "Finite field irreducible polynomial coefficients")
                      {
                          Argument = new Argument<int[]> {Arity = ArgumentArity.OneOrMore}
                      }
                  };
        }

        public Command ToCommand()
        {
            var command = new Command(Name, Description);
            foreach (var option in Options) command.AddOption(option);
            command.Handler = CommandHandler.Create(GetType().GetMethod("Execute"), this);

            return command;
        }
    }
}