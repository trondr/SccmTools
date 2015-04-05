using System.Windows.Controls;

namespace SccmTools.Library.Common.UI
{
    public class ViewBase: UserControl
    {
        private ViewModelBase _viewModel;

        protected ViewBase()
        {
            //Loaded+=OnLoaded;
        }

        public ViewModelBase ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                this.DataContext = _viewModel;
            }
        }

        //private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    this.DataContext = this.ViewModel;            
        //}
    }
}
