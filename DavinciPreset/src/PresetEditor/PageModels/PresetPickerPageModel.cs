using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using System.Collections.ObjectModel;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.PageModels
{
    public partial class PresetPickerPageModel : ObservableObject
    {
        private readonly IPresetSettingSegment _presetSettingSegment;
        private readonly IPopupService _popupService;
        
        private string _settingContent = string.Empty;
        private InstanceInput? _draggedTile;

        [ObservableProperty] private NodeMenuItem _selectedNodeMenuItem;
        [ObservableProperty] private ObservableCollection<NodeMenuItem> _nodeMenuItems;

        [ObservableProperty] private string? _filePath;
        [ObservableProperty] private string _groupNode = string.Empty;
        [ObservableProperty] private bool _isMarkGroup = false;
        [ObservableProperty] private bool _isMarkTab = false;
        [ObservableProperty] private string _groupSource;
        [ObservableProperty] private ObservableCollection<InstanceInput> _instanceInputs = [];
        [ObservableProperty] private ObservableCollection<object> _selectedItems = [];
        
        [ObservableProperty] private ObservableCollection<GroupInput> _groupInputs = [];

        private IEnumerable<string> InputNames = [];
        [ObservableProperty] private ObservableCollection<string> _moveInputNames = [];
        [ObservableProperty] private ObservableCollection<string> _filteredInputNames = [];
        [ObservableProperty] private string _baseInputName;
        
        public PresetPickerPageModel(IPresetSettingSegment presetSettingSegment, IPopupService popupService)
        {
            _popupService = popupService;
            _presetSettingSegment = presetSettingSegment;

            NodeMenuItems =
            [
                new NodeMenuItem { Id = NodeType.GroupNode, Title = "分组节点" },
                new NodeMenuItem { Id = NodeType.PresetNode, Title = "预设节点" }
            ];
            SelectedNodeMenuItem = NodeMenuItems[0];
            SelectedNodeMenuItem.BackColor = Color.FromArgb("#046BC7");
        }
        
        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task NodeMenuItemsSelectionChanged(NodeMenuItem item)
        {
            if (item == null || SelectedNodeMenuItem == item) return Task.CompletedTask;
            SelectedNodeMenuItem = item;

            item.BackColor = Color.FromArgb("#046BC7");
            var unSelectedItems = NodeMenuItems.Where(x => x != item);
            foreach (var unSelectedItem in unSelectedItems)
                unSelectedItem.BackColor = Colors.Transparent;
            
            return Task.CompletedTask;
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
                    InstanceInputs.Add(new InstanceInput
                    {
                        InputName = instanceInputRaw.InputName,
                        PropertyList = new ObservableCollection<InputItem>(instanceInputRaw.PropertyList)
                    });
                }

                InputNames = InstanceInputs.Select(i => i.InputName);
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
            if (!selectedItems.Contains(_draggedTile))
            {
                _draggedTile = null;
                await Toast.Make("Dragged Item must be one of the Selected Items").Show();
                return;
            }
            
            var newIndex = InstanceInputs.IndexOf(tile);
            var selectedIndexes = selectedItems.Select(x => InstanceInputs.IndexOf(x!));
            foreach (var selectedIndex in selectedIndexes.Reverse())
               InstanceInputs.Move(selectedIndex, newIndex);

            SelectedItems.Clear();
            _draggedTile = null;
        }

        [RelayCommand]
        private async Task OnInstanceInputTap(InstanceInput tile)
        {
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(InstanceInputViewModel.InstanceInput)] = tile
            };
            var popupOptions = new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            await _popupService.ShowPopupAsync<InstanceInputViewModel>(
            Shell.Current,
            options: popupOptions,
            shellParameters: queryAttributes);

            var instanceInput = InstanceInputs.First(x => x.InputName == tile.InputName);
            instanceInput.InputName = tile.InputName;
            instanceInput.PropertyList = tile.PropertyList;

            OnPropertyChanged(nameof(InstanceInputs));
        }

        [RelayCommand]
        private void InputNameTextChanged(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return;
            FilteredInputNames.Clear();
            FilteredInputNames.AddRange(InputNames.Where(i => i.Contains(filter, StringComparison.CurrentCultureIgnoreCase)));
        }

        [RelayCommand]
        private void DeleteMoveInputName(string inputName)
        {
            MoveInputNames.Remove(inputName);
        }

        [RelayCommand]
        private async Task Move()
        {
            if (string.IsNullOrWhiteSpace(BaseInputName))
                await Toast.Make("请选择基准InstanceInput", ToastDuration.Short, 20).Show();
            if (MoveInputNames.Count == 0)
                await Toast.Make("请选择需要移动的InstanceInputs", ToastDuration.Short, 20).Show();

            var baseIndex = InstanceInputs.IndexOf(InstanceInputs.First(i => i.InputName == BaseInputName));
            foreach (var moveInputName in MoveInputNames.Reverse())
            {
                var moveInput = InstanceInputs.FirstOrDefault(i => i.InputName == moveInputName);
                if (moveInput == null) continue;
                var moveIndex = InstanceInputs.IndexOf(moveInput);
                InstanceInputs.Move(moveIndex,baseIndex);
            }
        }
    }
}
