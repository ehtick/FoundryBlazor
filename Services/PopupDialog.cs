
using FoundryBlazor.Message;
using Radzen;
namespace FoundryBlazor.Shared;

public interface IPopupDialog
{

    bool Open();
    bool Alert(string message, string title);
}

public class PopupDialog : IPopupDialog
{
    private DialogService? _dialogService { get; set; }


    public PopupDialog(DialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public bool Open()
    {
        return _dialogService?.Alert("Hello World", "Popup Dialog");
    }
    public bool Alert(string message, string title)
    {
        return _dialogService?.Alert(message, title);
    }


}