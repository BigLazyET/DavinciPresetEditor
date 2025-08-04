using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using System.Collections.ObjectModel;

namespace PresetEditor.PageModels
{
    public partial class PresetPickerPageModel(IPresetSettingSegment presetSettingSegment, IPopupService popupService) : ObservableObject
    {
        private InstanceInput? _draggedTile;

        [ObservableProperty] private string? _filePath;
        [ObservableProperty] private ObservableCollection<InstanceInput> _instanceInputs = [];
        [ObservableProperty] private IList<InstanceInput> _selectedItems = [];

        [RelayCommand]
        private async Task<FileResult?> OnPickFile()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result == null || !result.FileName.EndsWith("setting", StringComparison.OrdinalIgnoreCase)) return result;
                FilePath = result.FullPath;
                await using var stream = await result.OpenReadAsync();
                using var sr = new StreamReader(stream);
                var text = await sr.ReadToEndAsync();
                var json = presetSettingSegment.OrderedInputs2Json(text);
                var instanceInputs = presetSettingSegment.OrderedInputs2List(json);
                if (instanceInputs == null) return null;
                InstanceInputs = new ObservableCollection<InstanceInput>(instanceInputs);

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        [RelayCommand]
        private Task OnDragStarting(InstanceInput tile)
        {
            _draggedTile = tile;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task OnInstanceInputDrop(InstanceInput tile)
        {
            if (_draggedTile == null) return;
            if (!SelectedItems.Contains(_draggedTile))
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                string text = "Dragged Item must be one of the Selected Items";
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 14;

                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
                return;
            }
            
            var newIndex = InstanceInputs.IndexOf(tile);
            var selectedIndexs = SelectedItems.Select(x => InstanceInputs.IndexOf(x));
            
            foreach (var selectedIndex in selectedIndexs)
            {
                InstanceInputs.Move(selectedIndex, newIndex);
            }
            SelectedItems.Clear();
            _draggedTile = null;
            
            return;
        }

        [RelayCommand]
        private async Task OnInstanceInputTap(InstanceInput tile)
        {
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(InstanceInputViewModel.InstanceInput)] = tile
            };
            await popupService.ShowPopupAsync<InstanceInputViewModel>(
            Shell.Current,
            options: PopupOptions.Empty,
            shellParameters: queryAttributes);
        }
    }
}
