using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Localizations;
using PresetEditor.Models;

namespace PresetEditor.ViewModels;

public partial class GroupNodeEditPopupViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private GroupInput _groupInput;
    
    private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
    private string ConfirmD => LocalizationResourceManager.Instance["Confirm"].ToString();
    private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        GroupInput = (GroupInput)query[nameof(GroupInput)];
    }
    
    [RelayCommand]
    private async Task Delete(InputItem item)
    {
        if (GroupInput.PropertyList.Count == 0) return;
        var res=  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["DeleteRemind"].ToString(),ConfirmD, Cancel);
        if (!res) return;
        GroupInput.PropertyList.Remove(item);
    }
}