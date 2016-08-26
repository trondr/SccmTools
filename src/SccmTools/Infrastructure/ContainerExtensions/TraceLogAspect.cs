using System;
using Castle.DynamicProxy;
using Common.Logging;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    public class TraceLogAspect : IInterceptor
    {
        private readonly IInvocationLogStringBuilder _invocationLogStringBuilder;
        private readonly ILogFactory _logFactory;
        private ILog _logger;

        public TraceLogAspect(IInvocationLogStringBuilder invocationLogStringBuilder, ILogFactory logFactory)
        {
            _invocationLogStringBuilder = invocationLogStringBuilder;
            _logFactory = logFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            _logger = _logFactory.GetLogger(invocation.TargetType);
            if (_logger.IsTraceEnabled) _logger.Trace(_invocationLogStringBuilder.BuildLogString(invocation, InvocationPhase.Start));
            try
            {
                invocation.Proceed();
                if (_logger.IsTraceEnabled) _logger.Trace(_invocationLogStringBuilder.BuildLogString(invocation, InvocationPhase.End));
            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled) _logger.Error(_invocationLogStringBuilder.BuildLogString(invocation, InvocationPhase.Error), ex);
                throw;
            }
        }
    }
}