using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;
using TaskManagerWPF.Data;
using TaskManagerWPF.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Data;
namespace TaskManagerWPF.ViewModels;


public class TaskViewModel : BaseViewModel
{
    public ICommand AddCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
    public Array Statusuri => Enum.GetValues(typeof(StatusTask));
    public Array Categorii => Enum.GetValues(typeof(CategoriiTask));
    public Array Prioritati => Enum.GetValues(typeof(PrioritateTask));
    public List<FilterOption<StatusTask>> StatusuriFiltru { get; }
    public List<FilterOption<CategoriiTask>> CategoriiFiltru { get; }
    public List<FilterOption<PrioritateTask>> PrioritatiFiltru { get; }

    private ObservableCollection<TaskModel> _taskuriC = new();

    public ICollectionView TaskuriView {get; private set; }

    private readonly IDialogService _dialogService;
    private string? _searchText;

    public string? SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            TaskuriView?.Refresh();
        }
    }
    
    private FilterOption<StatusTask>? _selectedStatus;
    public FilterOption<StatusTask>? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
            TaskuriView.Refresh();
        }
    }
    private FilterOption<CategoriiTask>? _selectedCategorie;
    public FilterOption<CategoriiTask>? SelectedCategorie
    {
        get => _selectedCategorie;
        set
        {
            _selectedCategorie = value;
            OnPropertyChanged();
            TaskuriView.Refresh();
        }
    }
    private FilterOption<PrioritateTask>? _selectedPrioritate;
    public FilterOption<PrioritateTask>? SelectedPrioritate
    {
        get => _selectedPrioritate;
        set
        {
            _selectedPrioritate = value;
            OnPropertyChanged();
            TaskuriView.Refresh();
        }
    }
    public ObservableCollection<TaskModel> TaskuriC
    {
        get => _taskuriC;
        set
        {
            _taskuriC = value;
            OnPropertyChanged();
        }
    }
    
    

    //task curent pentru editare sau adaugare
    private TaskModel _selectedTask;

    public TaskModel SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            OnPropertyChanged();
           
           
            if (_selectedTask != null)
            {
                CurrentTask = new TaskModel
                {
                    Id = _selectedTask.Id,
                    Titlu = _selectedTask.Titlu,
                    Descriere = _selectedTask.Descriere,
                    Deadline = _selectedTask.Deadline,
                    Categorie = _selectedTask.Categorie,
                    Status = _selectedTask.Status,
                    Prioritate = _selectedTask.Prioritate
                };
            } 
            OnPropertyChanged(nameof(Ora));
            OnPropertyChanged(nameof(Minut));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private TaskModel _currentTask = new TaskModel();

    public TaskModel CurrentTask
    {
        get => _currentTask;
        set
        {
            if (_currentTask != null)
                _currentTask.PropertyChanged -= CurrentTask_PropertyChanged;

            _currentTask = value;

            if (_currentTask != null)
                _currentTask.PropertyChanged += CurrentTask_PropertyChanged;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }
    private void CurrentTask_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
        if (e.PropertyName == nameof(TaskModel.Deadline))
        {
            OnPropertyChanged(nameof(Ora));
            OnPropertyChanged(nameof(Minut));
        }
    }



    //selectare deadline
    public int Ora
    {
        get => CurrentTask?.Deadline?.Hour ?? 0;
        set
        {
            if (CurrentTask?.Deadline == null) return;

            var d = CurrentTask.Deadline.Value;

            CurrentTask.Deadline = new DateTime(
                d.Year,
                d.Month,
                d.Day,
                value,
                d.Minute,
                0);

            OnPropertyChanged(nameof(Ora));
            OnPropertyChanged(nameof(CurrentTask));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public int Minut
    {
        get => CurrentTask?.Deadline?.Minute ?? 0;
        set
        {
            if (CurrentTask?.Deadline == null) return;

            var d = CurrentTask.Deadline.Value;

            CurrentTask.Deadline = new DateTime(
                d.Year,
                d.Month,
                d.Day,
                d.Hour,
                value,
                0);

            OnPropertyChanged(nameof(Minut));
            OnPropertyChanged(nameof(CurrentTask));
            CommandManager.InvalidateRequerySuggested();
        }
    }
    

    //constructor
    public TaskViewModel()
    {
        _dialogService = new DialogService();
        
        CurrentTask = new TaskModel(); 
        
        TaskuriView=CollectionViewSource.GetDefaultView(TaskuriC);
        TaskuriView.Filter = FiltruTaskuri;
        TaskuriView.SortDescriptions.Add(
            new SortDescription(nameof(TaskModel.UrgentaSortare), ListSortDirection.Ascending));

        TaskuriView.SortDescriptions.Add(
            new SortDescription(nameof(TaskModel.PrioritateSortare), ListSortDirection.Ascending));

        TaskuriView.SortDescriptions.Add(
            new SortDescription(nameof(TaskModel.Deadline), ListSortDirection.Ascending));
        
        IncarcaTaskuri();
        
        StatusuriFiltru = new List<FilterOption<StatusTask>>
        {
            new FilterOption<StatusTask>{ Display="Toate", Value= null }
        };

        StatusuriFiltru.AddRange(
            Enum.GetValues(typeof(StatusTask))
                .Cast<StatusTask>()
                .Select(s => new FilterOption<StatusTask>
                {
                    Display = s.ToString(),
                    Value = s
                }));

        CategoriiFiltru = new List<FilterOption<CategoriiTask>>
        {
            new FilterOption<CategoriiTask>{ Display="Toate", Value=null }
        };
        
        CategoriiFiltru.AddRange(
            Enum.GetValues(typeof(CategoriiTask))
                .Cast<CategoriiTask>()
                .Select(c => new FilterOption<CategoriiTask>
                {
                    Display = c.ToString(),
                    Value = c
                }));


        PrioritatiFiltru = new List<FilterOption<PrioritateTask>>
        {
            new FilterOption<PrioritateTask>{ Display="Toate", Value=null }
        };

        PrioritatiFiltru.AddRange(
            Enum.GetValues(typeof(PrioritateTask))
                .Cast<PrioritateTask>()
                .Select(p => new FilterOption<PrioritateTask>
                {
                    Display = p.ToString(),
                    Value = p
                }));
        
        AddCommand = new RelayCommand(_ => SaveTask(), _ => IsTaskValid());
        DeleteCommand = new RelayCommand(_ => DeleteSelected(), _ => SelectedTask != null);

        CommandManager.InvalidateRequerySuggested(); 
    }

    //Stergere
    private void DeleteSelected()
    {
        if(SelectedTask==null) return;
        
        bool rezultat = _dialogService.ShowConfirmation(
            $"Sigur vrei sa stergi task-ul \"{SelectedTask.Titlu}\"?",
            "Confirmare");

        if (!rezultat) return;
        using var connection=DatabaseHelper.GetConnection();
        string query = "DELETE FROM Taskuri WHERE Id=@id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@id", SelectedTask.Id);
        command.ExecuteNonQuery();

        IncarcaTaskuri();
        TaskuriView.Refresh();

        CurrentTask = new TaskModel { Deadline = DateTime.Now };

        OnPropertyChanged(nameof(Ora));
        OnPropertyChanged(nameof(Minut));
        

    }
    

    
    //Adaugare si actualizare
    public void SaveTask()
    {
        using var connection = DatabaseHelper.GetConnection();
        
        if (CurrentTask.Id == 0)
        {
            // INSERT
            string insert = @"INSERT INTO Taskuri
        (Titlu, Descriere, Deadline, Categorie, Status, Prioritate)
        VALUES (@Titlu,@Descriere,@Deadline,@Categorie,@Status,@Prioritate)";

            using var cmd = new SQLiteCommand(insert, connection);

            cmd.Parameters.AddWithValue("@Titlu", CurrentTask.Titlu);
            cmd.Parameters.AddWithValue("@Descriere", CurrentTask.Descriere);
            cmd.Parameters.AddWithValue("@Deadline", CurrentTask.Deadline.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Categorie", (int)CurrentTask.Categorie);
            cmd.Parameters.AddWithValue("@Status",  (int)CurrentTask.Status);
            cmd.Parameters.AddWithValue("@Prioritate", (int)CurrentTask.Prioritate);

            cmd.ExecuteNonQuery();
        }
        else
        {
            // UPDATE
            string update = @"UPDATE Taskuri SET
        Titlu=@Titlu,
        Descriere=@Descriere,
        Deadline=@Deadline,
        Categorie=@Categorie,
        Status=@Status,
        Prioritate=@Prioritate
        WHERE Id=@Id";

            using var cmd = new SQLiteCommand(update, connection);

            cmd.Parameters.AddWithValue("@Titlu", CurrentTask.Titlu);
            cmd.Parameters.AddWithValue("@Descriere", CurrentTask.Descriere);
            cmd.Parameters.AddWithValue("@Deadline", CurrentTask.Deadline.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Categorie",(int)CurrentTask.Categorie);
            cmd.Parameters.AddWithValue("@Status", (int)CurrentTask.Status);
            cmd.Parameters.AddWithValue("@Prioritate", (int)CurrentTask.Prioritate);
            cmd.Parameters.AddWithValue("@Id", CurrentTask.Id);

            cmd.ExecuteNonQuery();
        }

        IncarcaTaskuri();
        TaskuriView.Refresh();


        // RESET FORMULAR
        CurrentTask = new TaskModel { Deadline = DateTime.Now };
        OnPropertyChanged(nameof(CurrentTask));
        OnPropertyChanged(nameof(Ora));
        OnPropertyChanged(nameof(Minut));

    }


    //Obtinere Taskuri
    public List<TaskModel> ObtinereTaskuri()
    {
        List<TaskModel> taskuri = new List<TaskModel>();
        using (var connection = DatabaseHelper.GetConnection())
        {
            string selectQuery = "SELECT * FROM Taskuri";
            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"Coloana {i}: {reader.GetName(i)}");
                        }

                        int id = reader.GetInt32(0);
                        string titlu = reader.GetString(1);
                        string descriere = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        DateTime? deadline = null;

                        var rawDeadline = reader.GetValue(3);

                        if (rawDeadline != DBNull.Value)
                        {
                            if (rawDeadline is string s)
                                deadline = DateTime.Parse(s);
                            else
                                deadline = Convert.ToDateTime(rawDeadline);
                        }
                        CategoriiTask categorie = (CategoriiTask)reader.GetInt32(4);
                        StatusTask status = (StatusTask)reader.GetInt32(5);
                        PrioritateTask prioritate = (PrioritateTask)reader.GetInt32(6);
                        TaskModel task = new TaskModel
                        {
                            Titlu = titlu,
                            Descriere = descriere,
                            Deadline = deadline,
                            Categorie = categorie,
                            Status = status,
                            Prioritate = prioritate
                        };
                        task.Id = id;
                        taskuri.Add(task);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Eroare la citirea taskului: {ex.Message}");
                    }
                }
            }
        }

        return taskuri;
    }


    //Sterge Task
    public void StergeTask(int id)
    {
        using var connection = DatabaseHelper.GetConnection();

        string query = "DELETE FROM Taskuri WHERE Id=@id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();

        IncarcaTaskuri();
    }


    //Validare
    private bool IsTaskValid()
    {
        if (CurrentTask == null)
            return false;

        if (!string.IsNullOrEmpty(CurrentTask[nameof(CurrentTask.Titlu)]))
            return false;

        if (!string.IsNullOrEmpty(CurrentTask[nameof(CurrentTask.Deadline)]))
            return false;

        if (!string.IsNullOrEmpty(CurrentTask[nameof(CurrentTask.Descriere)]))
            return false;

        return true;
    }
    
    public void IncarcaTaskuri()
    {
        TaskuriC.Clear();
        var listaDinBD = ObtinereTaskuri();
        foreach (var task in listaDinBD)
        {
            TaskuriC.Add(task);
        }
    }

    private bool FiltruTaskuri(object obj)
    {
        if (obj is not TaskModel task)
            return false;
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            if (task.Titlu == null ||
                !task.Titlu.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return false;
        }
        bool noStatus = SelectedStatus?.Value == null;
        bool noCat = SelectedCategorie?.Value == null;
        bool noPrio = SelectedPrioritate?.Value == null;

        if (noStatus && noCat && noPrio)
            return true;

        if (SelectedStatus?.Value != null && task.Status != SelectedStatus.Value)
            return false;
        
        if (SelectedCategorie?.Value != null && task.Categorie != SelectedCategorie.Value)
            return false;
        
        if (SelectedPrioritate?.Value != null && task.Prioritate != SelectedPrioritate.Value)
            return false;

        return true;
    }


    
}
   

    