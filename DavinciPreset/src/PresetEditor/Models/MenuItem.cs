using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PresetEditor.Models;

public class MenuItem: INotifyPropertyChanged
{
    private string _icon;
    private string _title;
    private string _textColor;
    private string _description;
    private bool _hasNotification;
    private Color _backColor = Colors.Transparent;

    public string Icon
    {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string TextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }
    
    public Color BackColor
    {
        get => _backColor;
        set => SetProperty(ref _backColor, value);
    }

    public string Description
    {
        get => _description; 
        set => SetProperty(ref _description, value); 
    }

    public bool HasNotification
    {
        get => _hasNotification; 
        set => SetProperty(ref _hasNotification, value); 
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }
        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}