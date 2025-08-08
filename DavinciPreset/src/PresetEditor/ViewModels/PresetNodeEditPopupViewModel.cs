using CommunityToolkit.Mvvm.ComponentModel;
using PresetEditor.Models;

namespace PresetEditor.ViewModels
{
    public partial class PresetNodeEditPopupViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private InstanceInput _instanceInput;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            InstanceInput = (InstanceInput)query[nameof(InstanceInput)];
        }
    }
}
