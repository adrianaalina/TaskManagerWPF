using System;
using System.ComponentModel;
using System.Windows.Media;
namespace TaskManagerWPF.Models;
using System.ComponentModel;

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
public class TaskModel : INotifyPropertyChanged, IDataErrorInfo
{
    private int id;
    private string? titlu;
    private string? descriere;
    private DateTime? deadline;
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
    
    public DateTime? Deadline
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
    
    public int PrioritateSortare =>
        Prioritate switch
        {
            PrioritateTask.Urgent => 0,
            PrioritateTask.Normal => 1,
            PrioritateTask.Optional => 2,
            _ => 3
        };
    public int UrgentaSortare
    {
        get
        {
            if (!Deadline.HasValue)
                return 3;

            if (Deadline.Value < DateTime.Now)
                return 0; // intarziat

            if (Deadline.Value.Date == DateTime.Now.Date)
                return 1; // azi

            return 2; // pentru viitor
        }
    }

    
    protected void OnPropertyChanged(string numeProprietate)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(numeProprietate));
    }

    public bool EsteIntarziat => Deadline.HasValue && Deadline.Value < DateTime.Now;
    public bool EsteAzi =>
        Deadline.HasValue &&
        Deadline.Value.Date == DateTime.Now.Date &&
        Deadline.Value > DateTime.Now;
    public string Error => null;

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(Titlu):
                    if (string.IsNullOrWhiteSpace(Titlu))
                        return "Titlul este obligatoriu.";
                    if (Titlu.Length < 3)
                        return "Titlul trebuie sa aiba minim 3 caractere.";
                    break;

                case nameof(Deadline):
                    if (Deadline == null)
                        return "Selecteaza un deadline.";
                    if (Deadline < DateTime.Now.Date)
                        return "Nu poti seta un deadline in trecut.";
                    break;

                case nameof(Descriere):
                    if (string.IsNullOrWhiteSpace(Descriere))
                        return "Descrierea este obligatorie.";
                    if (Descriere.Length < 5)
                        return "Descrierea trebuie sa aiba minim 5 caractere.";
                    break;
            }

            return null;
        }
    }
    
}