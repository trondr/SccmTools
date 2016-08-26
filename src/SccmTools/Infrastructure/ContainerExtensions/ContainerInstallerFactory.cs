//Credits: http://stackoverflow.com/questions/6358206/how-to-force-the-order-of-installer-execution

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor.Installer;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    public class ContainerInstallerFactory : InstallerFactory
    {
        public override IEnumerable<Type> Select(IEnumerable<Type> installerTypes)
        {
            var sortedInstallerTypes = installerTypes.OrderBy(this.GetPriority);            
            return sortedInstallerTypes;
        }

        private int GetPriority(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(InstallerPriorityAttribute), false).FirstOrDefault() as InstallerPriorityAttribute;
            return attribute?.Priority ?? InstallerPriorityAttribute.DefaultPriority;
        }
    }
}

