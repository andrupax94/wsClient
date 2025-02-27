public class StatusMessageService
{
    private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnStatusMessageChanged();
            }
        }
    }

    public event Action StatusMessageChanged;

    protected virtual void OnStatusMessageChanged()
    {
        StatusMessageChanged?.Invoke();
    }

}