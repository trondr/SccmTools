using System;
using Castle.DynamicProxy;
using Common.Logging;

namespace SccmTools.Infrastructure
{
    public class DebugLogAspect : IInterceptor
    {
        private readonly IInvocationLogStringBuilder _invocationLogStringBuilder;
        private readonly ILogFactory _logFactory;
        private ILog _logger;

        public DebugLogAspect(IInvocationLogStringBuilder invocationLogStringBuilder, ILogFactory logFactory)
        {
            _invocationLogStringBuilder = invocationLogStringBuilder;
            _logFactory = logFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            _logger = _logFactory.GetLogger(invocation.TargetType);
            if (_logger.IsDebugEnabled) _logger.Debug(_invocationLogStringBuilder.BuildLogString(invocation, InvocationPhase.Start) );
            try
            {
                invocation.Proceed();
                if (_logger.IsDebugEnabled) _logger.Debug(_invocationLogStringBuilder.BuildLogString(invocation, InvocationPhase.End));
            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled) _logger.Error(_invocationLogStringBuilder.BuildLogString(invocation, InvocationPhase.Error), ex);
                throw;
            }
        }        
    }
}
