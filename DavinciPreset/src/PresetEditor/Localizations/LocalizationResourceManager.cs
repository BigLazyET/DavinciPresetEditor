using System.ComponentModel;
using System.Globalization;
using PresetEditor.Resources.Languages;

namespace PresetEditor.Localizations;

public class LocalizationResourceManager : INotifyPropertyChanged {
    
    public Action OnCultureChanged;
    
    private LocalizationResourceManager() {
        Culture = CultureInfo.CurrentCulture;
    }

    public CultureInfo Culture
    {
        get => CultureInfo.CurrentUICulture;
        set
        {
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            OnCultureChanged?.Invoke();
        }
    }

    public static LocalizationResourceManager Instance { get; } = new();

    public object this[string resourceKey]
        => AppResources.ResourceManager.GetObject(resourceKey, Culture) ?? Array.Empty<byte>();

    public event PropertyChangedEventHandler PropertyChanged;
}