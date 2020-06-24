namespace SP.Service.Background
{
    public enum BackgroundServiceStatus : int
    {
        NotFound = 0,
        Created = 1,
        Running = 2,
        RanToCompletion = 3,
        Cancelled = 4,
        Faulted = 5
    }
}
