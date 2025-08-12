using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.PageModels
{
    public partial class PresetPickerPageModel : ObservableObject
    {
        private readonly IPresetSettingSegment _presetSettingSegment;
        private readonly IPopupService _popupService;
        
        private string _settingContent = string.Empty;

        [ObservableProperty] private NodeMenuItem _selectedNodeMenuItem;
        [ObservableProperty] private ObservableCollection<NodeMenuItem> _nodeMenuItems;

        [ObservableProperty] private string? _filePath;
        
        [ObservableProperty] private bool _isMarkGroup = false;
        [ObservableProperty] private bool _isMarkTab = false;
        [ObservableProperty] private ObservableCollection<InstanceInput> _instanceInputs = [];
        [ObservableProperty] private ObservableCollection<object> _selectedInstanceInputs = [];

        [ObservableProperty] private string _groupSourceOp;
        [ObservableProperty] private string _groupSourceOpType;
        [ObservableProperty] private ObservableCollection<GroupInput> _groupInputs = [];
        [ObservableProperty] private GroupInput? _selectedGroupInput;

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

            SelectedGroupInput = null;
            SelectedInstanceInputs.Clear();
            
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
        private async Task GroupCollect()
        {
            if (string.IsNullOrWhiteSpace(GroupSourceOp))
            {
                await Toast.Make("分组节点名不能为空").Show();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(GroupSourceOpType))
            {
                await Toast.Make("分组节点类型不能为空").Show();
                return;
            }
            
            var groupSegment = $"{GroupSourceOp} = {GroupSourceOpType}";
            var groupInputs = _presetSettingSegment.GetGroupInputs(_settingContent, groupSegment);
            if (groupInputs == null) return;
            foreach (var groupInput in groupInputs)
                GroupInputs.Add(groupInput);
        }

        [RelayCommand]
        private async Task AddGroupInput()
        {
            if (string.IsNullOrWhiteSpace(GroupSourceOp))
            {
                await Toast.Make("分组节点名不能为空").Show();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(GroupSourceOpType))
            {
                await Toast.Make("分组节点类型不能为空").Show();
                return;
            }
            
            var groupInput = new GroupInput
            {
                PropertyList =
                [
                    new InputItem { Key = "LBLC_DropDownButton", Value = "true" },
                    new InputItem { Key = "INPID_InputControl", Value = "LabelControl" },
                    new InputItem { Key = "LBLC_NumInputs", Value = "" },
                    new InputItem { Key = "LINKS_Name", Value = "" },
                    new InputItem { Key = "INP_Default", Value = "1" },
                ]
            };
            
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(GroupNodeEditPopupViewModel.GroupInput)] = groupInput
            };
            var popupOptions = new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            var result = await _popupService.ShowPopupAsync<GroupNodeEditPopupViewModel, GroupInput>(
                Shell.Current,
                options: popupOptions,
                shellParameters: queryAttributes);

            if (result?.Result is not GroupInput resultGroupInput) return;
            
            var groupSourceNames = GroupInputs.Select(g => g.GroupSourceName);
            if (groupSourceNames.Contains(resultGroupInput.GroupSourceName))
            {
                await Toast.Make("分组名重复，添加失败").Show();
                return;
            }
            GroupInputs.Add(resultGroupInput);
        }
        
        [RelayCommand]
        private async Task ModifyGroupInput()
        {
            if (SelectedGroupInput == null)
            {
                await App.Current.MainPage.DisplayAlert("提醒", "请先选中要修改的项","确认");
                return;
            }
            
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(GroupNodeEditPopupViewModel.GroupInput)] = SelectedGroupInput
            };
            var popupOptions = new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            var result = await _popupService.ShowPopupAsync<GroupNodeEditPopupViewModel, GroupInput>(
                Shell.Current,
                options: popupOptions,
                shellParameters: queryAttributes);
            
            if (result?.Result is not GroupInput resultGroupInput) return;

            SelectedGroupInput = null;
        }
        
        [RelayCommand]
        private async Task DeleteGroupInput()
        {
            if (SelectedGroupInput == null)
            {
                await App.Current.MainPage.DisplayAlert("提醒", "请先选中要删除的项","确认");
                return;
            }
            
            var res=  await App.Current.MainPage.DisplayAlert("警告", "确认要删除分组节点么?","确认", "取消");
            if (!res) return;
            GroupInputs.Remove(SelectedGroupInput);
        }

        [RelayCommand]
        private async Task ModifyInstanceInput()
        {
            if (SelectedInstanceInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("警告", "请选择一项才可编辑","确认");
                return;
            }
            
            if (SelectedInstanceInputs.Count > 1)
            {
                await App.Current.MainPage.DisplayAlert("警告", "选择项只有一项时才可编辑","确认");
                return;
            }

            var selectedItem = SelectedInstanceInputs.FirstOrDefault();
            if (selectedItem is not InstanceInput tile) return;
            
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(PresetNodeEditPopupViewModel.InstanceInput)] = tile
            };
            var popupOptions = new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            var result = await _popupService.ShowPopupAsync<PresetNodeEditPopupViewModel, InstanceInput>(
            Shell.Current,
            options: popupOptions,
            shellParameters: queryAttributes);
            
            if (result?.Result is not InstanceInput resultInstanceInput) return;
            
            var instanceInput = InstanceInputs.First(x => x.InputName == resultInstanceInput.InputName);
            instanceInput.PropertyListChanged();
            
            SelectedInstanceInputs.Clear();
        }

        [RelayCommand]
        private async Task DeleteInstanceInput()
        {
            var res =  await App.Current.MainPage.DisplayAlert("警告", "确认要删除选中的预设节点么?","确认", "取消");
            if (!res) return;
            foreach (var selectedItem in SelectedInstanceInputs)
            {
                if (selectedItem is not InstanceInput instanceInput) continue;
                InstanceInputs.Remove(instanceInput);
            }
            
            SelectedInstanceInputs.Clear();
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
            
            SelectedInstanceInputs.Clear();
        }

        [RelayCommand]
        private async Task SyncGroupInputs()
        {
            var res=  await App.Current.MainPage.DisplayAlert("确认", "⚠️同步只会添加预设几点中不存在的分组节点！确认要同步分组节点配置到预设节点么?","确认", "取消");
            if (!res) return;
            
            var groupSourceNames = GroupInputs.Select(g => g.GroupSourceName);   // Label0, Label1, ...
            var alreadyGroupSourceNames =
                InstanceInputs.Where(g =>
                        g.PropertyList.FirstOrDefault(p => p.Key == "SourceOp")?.Value.ToString() == GroupSourceOp)
                    .Select(g => g.PropertyList.FirstOrDefault(p => p.Key == "Source")?.Value?.ToString());
                    
            foreach (var groupSourceName in groupSourceNames)
            {
                if (alreadyGroupSourceNames == null || !alreadyGroupSourceNames.Any() || alreadyGroupSourceNames.Contains(groupSourceName)) continue;
                var instanceInput = new InstanceInput
                {
                    InputName = groupSourceName,
                    PropertyList =
                    [
                        new InputItem { Key = "SourceOp", Value = GroupSourceOp },
                        new InputItem { Key = "Source", Value = groupSourceName }
                    ]
                };
                InstanceInputs.Insert(0, instanceInput);
            }
            
            SelectedInstanceInputs.Clear();
        }

        [RelayCommand]
        private async Task ExportInstanceInputs()
        {
            if (InstanceInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("确认", "⚠️当前预设节点没有任何配置项","确认");
                return;
            }
            
            var pickResult = await FolderPicker.Default.PickAsync();
            if (pickResult == null) return;

            var saveFile = Path.Combine(pickResult.Folder.Path, "InstanceInputs.setting");
            if (string.IsNullOrWhiteSpace(saveFile)) return;
            var content = _presetSettingSegment.OrderedInputs2Text(InstanceInputs);
            await File.WriteAllTextAsync(saveFile, content);
            
            await App.Current.MainPage.DisplayAlert("通知", "InstanceInputs.setting成功","确认");
        }
        
        [RelayCommand]
        private async Task ExportGroupInputs()
        {
            if (GroupInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("确认", "⚠️当前分组节点没有任何配置项","确认");
                return;
            }
            
            var pickResult = await FolderPicker.Default.PickAsync();
            if (pickResult == null) return;

            var saveFile = Path.Combine(pickResult.Folder.Path, "GroupInputs.setting");
            if (string.IsNullOrWhiteSpace(saveFile)) return;
            var content = _presetSettingSegment.OrderedGroups2Text(GroupInputs);
            await File.WriteAllTextAsync(saveFile, content);
            
            await App.Current.MainPage.DisplayAlert("通知", "生成GroupInputs.setting成功","确认");
        }
    }
}
