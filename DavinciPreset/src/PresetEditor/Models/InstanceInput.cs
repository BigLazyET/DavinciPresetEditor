using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PresetEditor.Models
{
    public partial class InstanceInput : ObservableObject
    {
        [ObservableProperty] private string _inputName;

        [ObservableProperty] private Color _markColor = Colors.Transparent;

        [ObservableProperty] private ObservableCollection<InputItem> _propertyList = [];
    }

    public partial class InputItem : ObservableObject
    {
        [ObservableProperty] private string _Key;

        [ObservableProperty] private string _value;
    }
}
