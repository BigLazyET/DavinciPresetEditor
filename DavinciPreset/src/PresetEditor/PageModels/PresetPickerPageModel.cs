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
    public partial class PresetPickerPageModel : ObservableObject
    {
        private readonly IPresetSettingSegment _presetSettingSegment;
        private readonly IPopupService _popupService;
        
        private string _settingContent = string.Empty;
        private InstanceInput? _draggedTile;

        [ObservableProperty] private string? _filePath;
        [ObservableProperty] private string _groupNode = string.Empty;
        [ObservableProperty] private bool _isMarkGroup = false;
        [ObservableProperty] private bool _isMarkTab = false;
        [ObservableProperty] private string _groupSource;
        [ObservableProperty] private ObservableCollection<InstanceInput> _instanceInputs = [];
        [ObservableProperty] private ObservableCollection<object> _selectedItems = [];
        
        [ObservableProperty] private ObservableCollection<GroupInput> _groupInputs = [];

        [ObservableProperty] private ObservableCollection<string> _inputNames = [];
        

        /// <inheritdoc/>
        public PresetPickerPageModel(IPresetSettingSegment presetSettingSegment, IPopupService popupService)
        {
            _presetSettingSegment = presetSettingSegment;
            _popupService = popupService;

            InstanceInputs =
            [
                new InstanceInput
                {
                    InputName = "foo", PropertyList = new ObservableCollection<InputItem>
                    {
                        new() { Key = "fookey", Value = "foovalue" }
                    }
                }
            ];
        }

        [RelayCommand]
        private async Task OnPickFile()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result == null || !result.FileName.EndsWith("setting", StringComparison.OrdinalIgnoreCase)) return;
                FilePath = result.FullPath;
                await using var stream = await result.OpenReadAsync();
                using var sr = new StreamReader(stream);
                _settingContent = await sr.ReadToEndAsync();
                var instanceInputRaws = _presetSettingSegment.GetOrderedInstanceInputs(_settingContent);
                if (instanceInputRaws == null) return;
                
                InstanceInputs.Clear();
                foreach (var instanceInputRaw in instanceInputRaws)
                {
                    var input = new InstanceInput
                    {
                        InputName = instanceInputRaw.InputName,
                        PropertyList = new ObservableCollection<InputItem>(instanceInputRaw.PropertyList)
                    };
                    InstanceInputs.Add(input);
                }

                InputNames = new ObservableCollection<string>(InstanceInputs.Select(i => i.InputName));
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
        }

        [RelayCommand]
        private Task GroupCollect()
        {
            var groupInputs = _presetSettingSegment.GetGroupInputs(_settingContent, GroupNode);
            if (groupInputs == null) return Task.CompletedTask;
            foreach (var groupInput in groupInputs)
                GroupInputs.Add(groupInput);
            return Task.CompletedTask;
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
            var selectedItems = SelectedItems.Select(item => item as InstanceInput);
            var instanceInputs = selectedItems as InstanceInput[] ?? selectedItems.ToArray();
            if (!instanceInputs.Contains(_draggedTile))
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var text = "Dragged Item must be one of the Selected Items";
                var duration = ToastDuration.Short;
                var fontSize = 14;

                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
                return;
            }
            
            var newIndex = InstanceInputs.IndexOf(tile);
            var selectedIndexs = instanceInputs.Select(x => InstanceInputs.IndexOf(x!));
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
            await _popupService.ShowPopupAsync<InstanceInputViewModel>(
            Shell.Current,
            options: PopupOptions.Empty,
            shellParameters: queryAttributes);
        }
    }
}
