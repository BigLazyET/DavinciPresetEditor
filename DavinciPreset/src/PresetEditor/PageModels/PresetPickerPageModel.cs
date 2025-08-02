using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using PresetEditor.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PresetEditor.PageModels
{
    public partial class PresetPickerPageModel(IPresetSettingSegment presetSettingSegment, IPopupService popupService) : ObservableObject
    {
        private InstanceInput? _draggedTile;

        [ObservableProperty] private string? _filePath;
        [ObservableProperty] private ObservableCollection<InstanceInput> _instanceInputs = [];

        [RelayCommand]
        private async Task<FileResult> OnTap()
        {
            try
            {
                var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    //{ DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    //{ DevicePlatform.Android, new[] { "application/comics" } }, // MIME type
                    //{ DevicePlatform.WinUI, new[] { ".cbr", ".cbz" } }, // file extension
                    //{ DevicePlatform.Tizen, new[] { "*/*" } },
                    //{ DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // UTType values
                    { DevicePlatform.WinUI, new [] { ".setting" } }
                });

                PickOptions options = new()
                {
                    PickerTitle = "Please select a fusion preset file",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("setting", StringComparison.OrdinalIgnoreCase))
                    {
                        FilePath = result.FullPath;
                        using var stream = await result.OpenReadAsync();
                        using var sr = new StreamReader(stream);
                        var text = await sr.ReadToEndAsync();
                        var json = presetSettingSegment.OrderedInputs2Json(text);
                        var instanceInputs = presetSettingSegment.OrderedInputs2List(json);
                        if (instanceInputs == null) return null;
                        InstanceInputs = new ObservableCollection<InstanceInput>(instanceInputs);
                    }
                }

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
        private Task OnInstanceInputDrop(InstanceInput tile)
        {
            if (_draggedTile == null) return Task.CompletedTask;
            var oldIndex = InstanceInputs.IndexOf(_draggedTile);
            var newIndex = InstanceInputs.IndexOf(tile);
            InstanceInputs.Move(oldIndex, newIndex);
            _draggedTile = null;
            return Task.CompletedTask;
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
