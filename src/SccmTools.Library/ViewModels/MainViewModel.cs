using System;
using System.Windows;
using System.Windows.Input;
using SccmTools.Library.Common.UI;
using SccmTools.Library.Views;

namespace SccmTools.Library.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        public MainViewModel()
        {
            ProductDescriptionLabelText = "Product Description:";
            MaxLabelWidth = 200 ;
            OkCommand = new CommandHandler(this.Exit, true);
        }

        public static readonly DependencyProperty ProductDescriptionProperty = DependencyProperty.Register(
            "ProductDescription", typeof(string), typeof(MainViewModel), new PropertyMetadata(default(string)));

        public string ProductDescription
        {
            get { return (string)GetValue(ProductDescriptionProperty); }
            set { SetValue(ProductDescriptionProperty, value); }
        }

        public static readonly DependencyProperty ProductDescriptionLabelTextProperty = DependencyProperty.Register(
            "ProductDescriptionLabelText", typeof(string), typeof(MainViewModel), new PropertyMetadata(default(string)));

        public string ProductDescriptionLabelText
        {
            get { return (string)GetValue(ProductDescriptionLabelTextProperty); }
            set { SetValue(ProductDescriptionLabelTextProperty, value); }
        }

        public static readonly DependencyProperty MaxLabelWidthProperty = DependencyProperty.Register(
            "MaxLabelWidth", typeof(int), typeof(MainViewModel), new PropertyMetadata(default(int)));

        public int MaxLabelWidth
        {
            get { return (int)GetValue(MaxLabelWidthProperty); }
            set { SetValue(MaxLabelWidthProperty, value); }
        }

        public ICommand OkCommand { get; set; }

        private void Exit()
        {
            if (MainWindow != null)
            {
                MainWindow.Close();
            }
            else
            {
                throw new Exception("Unable to close main window because reference to the main window has not been set.");
            }
        }        
    }
}