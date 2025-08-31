using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Localizations;
using PresetEditor.Models;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.ViewModels
{
    public partial class PresetNodeEditPopupViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private InstanceInput _instanceInput;
        
        private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
        private string ConfirmD => LocalizationResourceManager.Instance["Confirm"].ToString();
        private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            InstanceInput = (InstanceInput)query[nameof(InstanceInput)];
        }

        [RelayCommand]
        private async Task Delete(InputItem item)
        {
            if (InstanceInput.PropertyList.Count == 0) return;
            var res=  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["DeleteRemind"].ToString(),ConfirmD, Cancel);
            if (!res) return;
            InstanceInput.PropertyList.Remove(item);
        }
    }
}
