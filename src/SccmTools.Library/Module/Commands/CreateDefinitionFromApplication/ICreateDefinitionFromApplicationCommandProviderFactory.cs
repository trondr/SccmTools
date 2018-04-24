namespace SccmTools.Library.Module.Commands.CreateDefinitionFromApplication
{
    public interface ICreateDefinitionFromApplicationCommandProviderFactory
    {
        ICreateDefinitionFromApplicationCommandProvider GetCreateDefinitionFromApplicationCommandProvider();

        void Release(ICreateDefinitionFromApplicationCommandProvider createDefinitionFromApplicationCommandProvider);
    }
}