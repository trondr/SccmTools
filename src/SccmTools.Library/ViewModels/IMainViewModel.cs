using System.Windows.Input;

namespace SccmTools.Library.ViewModels
{
    public interface IMainViewModel
    {
        int MaxLabelWidth { get; set; }
        string ProductDescription { get; set; }
        string ProductDescriptionLabelText { get; set; }
        ICommand OkCommand { get; set; }
    }
}
