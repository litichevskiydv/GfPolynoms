namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Linq;
    using GfPolynoms.GaloisFields;
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
                      new Option<GaloisField>("--field")
                      {
                          Required = true,
                          Description = "Finite field, format <field-order>:[irreducible polynomial coefficients comma separated]",
                          Argument = new Argument<GaloisField>(
                                         result =>
                                         {
                                             var parts = result.Tokens[0].Value.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                                             if (parts.Length == 0 || parts.Length > 2)
                                                 throw new ArgumentException("Field option format is incorrect");

                                             return GaloisField.Create(
                                                 int.Parse(parts[0]),
                                                 parts.Length == 2
                                                     ? parts[1].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(x => int.Parse(x.Trim())).ToArray()
                                                     : null
                                             );
                                         }
                                     )
                                     {
                                         Arity = ArgumentArity.ExactlyOne
                                     }

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