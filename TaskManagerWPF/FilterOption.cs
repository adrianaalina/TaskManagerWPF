namespace TaskManagerWPF.Models
{
    public class FilterOption<T> where T : struct
    {
        public string Display { get; set; }
        public T? Value { get; set; }

        public override string ToString() => Display;
    }
}