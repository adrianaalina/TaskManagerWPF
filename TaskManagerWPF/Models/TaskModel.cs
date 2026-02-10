using System;
using System.ComponentModel;
using System.Windows.Media;
namespace TaskManagerWPF.Models;

public enum StatusTask
{
    Neinceput,
    InLucru,
    Finalizat
}

public enum CategoriiTask
{
    Personal,
    Facultate,
    Profesional,
    Nespecificat
}
public enum PrioritateTask
{
    Urgent,
    Normal,
    Optional
}
public class TaskModel : INotifyPropertyChanged
{
    private int id;
    private string? titlu;
    private string? descriere;
    private DateTime deadline;
    private CategoriiTask categorie;
    private StatusTask status;
    private PrioritateTask prioritate;

    public int Id
    {
        get => id;
        set
        {
            id=value;
            OnPropertyChanged(nameof(Id));

        }
    }

    public string Titlu
    {
        get => titlu;
        set { titlu = value;  OnPropertyChanged(nameof(Titlu)); } 
    }

    public string Descriere
    {
        get => descriere;
        set{descriere=value; OnPropertyChanged(nameof(Descriere)); }
    }
    
    public DateTime Deadline
    {
        get => deadline;
        set { deadline = value; OnPropertyChanged(nameof(Deadline)); }
    }

    public CategoriiTask Categorie
    {
        get => categorie;
        set { categorie = value; OnPropertyChanged(nameof(Categorie)); }
    }

    public StatusTask Status
    {
        get => status;
        set { status = value; OnPropertyChanged(nameof(Status)); }
    }
    public PrioritateTask Prioritate
    {
        get => prioritate;
        set { prioritate = value; OnPropertyChanged(nameof(Prioritate)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string numeProprietate)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(numeProprietate));
    }
   

}