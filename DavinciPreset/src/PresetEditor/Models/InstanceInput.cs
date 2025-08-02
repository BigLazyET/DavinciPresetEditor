using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PresetEditor.Models
{
    public partial class InstanceInput : ObservableObject
    {
        [ObservableProperty]
        private string _inputName;

        [ObservableProperty]
        private ObservableCollection<InstanceInputItem> _propertyList = [];
    }

    public partial class InstanceInputItem : ObservableObject
    {
        [ObservableProperty]
        private string _Key;

        [ObservableProperty]
        private string _value;
    }
}
