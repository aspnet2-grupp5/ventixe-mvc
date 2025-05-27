namespace Application.Models
{
    public class EventResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class EventResult<T> : EventResult
    {
        public T? Result { get; set; }
    }
}