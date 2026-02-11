

namespace TaskManagerWPF.Services
{
    public interface IDialogService
    {
        void ShowMesssage(string message, string title);
        bool ShowConfirmation(string message, string title);
    }
}
