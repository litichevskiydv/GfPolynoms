namespace AppliedAlgebra.GfToolbox.CommandsDescriptions
{
    using System.CommandLine;

    public interface ICommandDescription
    {
        Command ToCommand();
    }
}