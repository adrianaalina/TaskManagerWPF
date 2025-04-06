using System;
using System.Windows.Media;
namespace TaskManagerWPF.Models;

public enum StatusTask
{
    Neinceput,
    InLucru,
    Finalizat
}
public class TaskModel
{
    public int Id{get; set;}
    public string Titlul {get; set;}
    public string Descriere {get; set;}
    public DateTime Deadline {get; set;}
    public StatusTask Status  {get; set;}
    public SolidColorBrush BackgroundColor
    {
        get
        {
            return (Deadline - DateTime.Now).TotalHours < 24 ? Brushes.Coral : Brushes.Wheat;
        }
    }

}