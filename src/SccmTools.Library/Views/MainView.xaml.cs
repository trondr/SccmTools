using SccmTools.Library.Common.UI;
using SccmTools.Library.ViewModels;

namespace SccmTools.Library.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ViewBase
    {
        public MainView(MainViewModel viewModel)
        {
            this.ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
