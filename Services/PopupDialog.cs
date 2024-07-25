
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
        _dialogService?.Alert("Open", "Open");
        return true;
    }
    public bool Alert(string message, string title)
    {
        _dialogService?.Alert(message, title);
        return true;
    }


}