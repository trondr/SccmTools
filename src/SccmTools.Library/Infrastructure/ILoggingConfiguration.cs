namespace SccmTools.Library.Infrastructure
{
    public interface ILoggingConfiguration
    {
        string LogDirectoryPath { get; set; }
        string LogFileName { get; set; }
        string LogLevel { get; set; }
    }
}
