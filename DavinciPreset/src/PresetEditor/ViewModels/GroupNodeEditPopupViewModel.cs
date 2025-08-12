using CommunityToolkit.Mvvm.ComponentModel;
using PresetEditor.Models;

namespace PresetEditor.ViewModels;

public partial class GroupNodeEditPopupViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private GroupInput _groupInput;
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        GroupInput = (GroupInput)query[nameof(GroupInput)];
    }
}