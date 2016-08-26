//Credits: http://stackoverflow.com/questions/6358206/how-to-force-the-order-of-installer-execution

using System;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    public sealed class InstallerPriorityAttribute : Attribute
    {
        public InstallerPriorityAttribute(int priority)
        {
            Priority = priority;
        }
        public const int DefaultPriority = 100;

        public int Priority { get; private set; }
    }
}