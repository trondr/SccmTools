using Castle.DynamicProxy;

namespace SccmTools.Infrastructure
{
    public interface IInvocationLogStringBuilder
    {
        string BuildLogString(IInvocation invocation, InvocationPhase invocationPhase);
    }

    public enum InvocationPhase
    {
        Start,
        End,
        Error
    }
}