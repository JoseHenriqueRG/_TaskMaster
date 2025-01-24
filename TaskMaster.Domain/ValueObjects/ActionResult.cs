namespace TaskMaster.Domain.ValueObjects
{
    public class ActionResult<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
