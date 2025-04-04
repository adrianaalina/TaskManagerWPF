using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Windows.Media;
using TaskManagerWPF.Models;
namespace TaskManagerWPF.ViewModels;


public class TaskViewModel
{
    public ObservableCollection<TaskModel> Taskuri { get; set; } = new ObservableCollection<TaskModel>();
    public TaskViewModel()
    {
        Taskuri.Add(new TaskModel { Titlul = "Test Task", Descriere = "Descriere test",  Deadline = DateTime.Now.AddDays(1), Finalizat = false  });
    }
    
    public void AdaugaTask(string titlu, string descriere, DateTime deadline)
    {
        Taskuri.Add(new TaskModel { Titlul = titlu, Descriere = descriere, Deadline = deadline, Finalizat = false });
    }

    
}