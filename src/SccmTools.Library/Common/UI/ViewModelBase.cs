using System.Windows;
using SccmTools.Library.Views;

namespace SccmTools.Library.Common.UI
{
    public abstract class ViewModelBase : DependencyObject
    {
        public MainWindow MainWindow { get; set; }
    }
}
