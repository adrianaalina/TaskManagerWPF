using System.Windows;
using TaskManagerWPF.Services;

namespace TaskManagerWPF
{
    public class DialogService : IDialogService
    {
        public void ShowMesssage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}