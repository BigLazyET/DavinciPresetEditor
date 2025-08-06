using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PresetEditor.Models;

public partial class GroupInput : ObservableObject
{
    [ObservableProperty] private string _nodeName;
    
    [ObservableProperty] private string _groupName;

    [ObservableProperty]
    private ObservableCollection<InputItem> _propertyList = [];
}