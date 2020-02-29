namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System.Collections.Generic;
    using System.CommandLine;

    public interface ICommandDescription
    {
        string Name { get; }

        string Description { get; }

        IReadOnlyList<Option> Options { get; }

        Command ToCommand();
    }
}