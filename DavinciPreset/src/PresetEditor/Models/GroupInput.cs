using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PresetEditor.Models;

public partial class GroupInput : ObservableObject
{
    [ObservableProperty] private string _groupSouceName;

    [ObservableProperty]
    private ObservableCollection<InputItem> _propertyList = [];
}