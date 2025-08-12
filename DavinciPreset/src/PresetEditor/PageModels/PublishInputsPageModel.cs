using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using zoft.MauiExtensions.Core.Extensions;
using Path = System.IO.Path;

namespace PresetEditor.PageModels;

public partial class PublishInputsPageModel : ObservableObject
{
    private readonly IPopupService _popupService;
    private readonly IPresetSettingSegment _presetSettingSegment;
    
    public IEnumerable<string> InputNames { get; private set; } = [];
    
    [ObservableProperty] private ObservableCollection<InstanceInput> _instanceInputs = [];
    [ObservableProperty] private ObservableCollection<object> _selectedInstanceInputs = [];
    
    [ObservableProperty] private string _baseInputName;
    [ObservableProperty] private ObservableCollection<string> _moveInputNames = [];
    [ObservableProperty] private ObservableCollection<string> _filteredInputNames = [];

    [ObservableProperty] private string _exportFilePath;

    [ObservableProperty] private bool _isLoading;

    public PublishInputsPageModel(IPopupService popupService, IPresetSettingSegment presetSettingSegment)
    {
        _popupService = popupService;
        _presetSettingSegment = presetSettingSegment;
    }

    [RelayCommand]
    private async Task Fetch()
    {
        try
        {
            var result = await App.Current.MainPage!.DisplayAlert("提醒", 
                $"⚠️确认初始加载公开参数配置么?{Environment.NewLine}(原先的所有配置和操作都将重置){Environment.NewLine}请确保提前在仪表盘页导入模板文件!", "确认","取消");
            if (!result) return;
            
            IsLoading = true;
            var dashboardPageModel = App.Current.ServiceProvider.GetRequiredService<DashboardPageModel>();
            var settingContent = dashboardPageModel.SettingContent;
            if (string.IsNullOrEmpty(settingContent)) return;

            var instanceInputRaws = _presetSettingSegment.GetOrderedInstanceInputs(settingContent);
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
            
            // 清空/重置状态
            SelectedInstanceInputs.Clear();
            MoveInputNames.Clear();
            FilteredInputNames.Clear();
            BaseInputName = string.Empty;
        }
        finally
        {
            IsLoading = false;
        }
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
        {
            await App.Current.MainPage!.DisplayAlert("提示", "⚠️请选择基准InstanceInput","确认");
            return;
        }

        if (MoveInputNames.Count == 0)
        {
            await App.Current.MainPage!.DisplayAlert("提示", "⚠️请选择需要移动的InstanceInputs","确认");
            return;
        }

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
    private async Task ModifyInstanceInput()
    {
        if (SelectedInstanceInputs.Count is 0 or > 1)
        {
            await App.Current.MainPage.DisplayAlert("提醒", "请选中一项进行编辑","确认");
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
            
        // var instanceInput = InstanceInputs.First(x => x.InputName == resultInstanceInput.InputName);
        // instanceInput.PropertyListChanged();
        resultInstanceInput.PropertyListChanged();
        // tile.PropertyListChanged();
        
    }

    [RelayCommand]
    private async Task DeleteInstanceInput()
    {
        if (SelectedInstanceInputs.Count is 0)
        {
            await App.Current.MainPage.DisplayAlert("提醒", "请选中一项或多项进行删除","确认");
            return;
        }
        
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
    private async Task SyncGroupInputs()
    {
        var groupSourcePageModel = App.Current.ServiceProvider.GetRequiredService<GroupSourcesPageModel>();
        var groupInputs = groupSourcePageModel.GroupInputs;
        var groupSourceOp = groupSourcePageModel.GroupSourceOp;
        if (groupInputs.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert("提醒", "分组源配置为空，请先配置分组源","确认");
            return;
        }

        if (string.IsNullOrWhiteSpace(groupSourceOp))
        {
            await App.Current.MainPage.DisplayAlert("提醒", "分组节点名为空，请先配置分组节点名","确认");
            return;
        }
        
        var res=  await App.Current.MainPage.DisplayAlert("提醒", "⚠️同步只会添加公开参数中不存在的分组源配置！确认要同步么?","确认", "取消");
        if (!res) return;
            
        var groupSourceNames = groupInputs.Select(g => g.GroupSourceName);   // Label0, Label1, ...
        var alreadyGroupSourceNames =
            InstanceInputs.Where(g =>
                    g.PropertyList.FirstOrDefault(p => p.Key == "SourceOp")?.Value.ToString() == groupSourceOp)
                .Select(g => g.PropertyList.FirstOrDefault(p => p.Key == "Source")?.Value?.ToString());
                    
        foreach (var groupSourceName in groupSourceNames.Reverse())
        {
            if (alreadyGroupSourceNames == null || !alreadyGroupSourceNames.Any() || alreadyGroupSourceNames.Contains(groupSourceName)) continue;
            var instanceInput = new InstanceInput
            {
                InputName = groupSourceName,
                PropertyList =
                [
                    new InputItem { Key = "SourceOp", Value = groupSourceOp },
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
            await App.Current.MainPage.DisplayAlert("提醒", "⚠️当前没有任何公开参数项","确认");
            return;
        }
            
        var pickResult = await FolderPicker.Default.PickAsync();
        if (pickResult?.Folder == null || string.IsNullOrWhiteSpace(pickResult.Folder.Path)) return;

        var saveFile = Path.Combine(pickResult.Folder.Path, "InstanceInputs.setting");
        var content = _presetSettingSegment.OrderedInputs2Text(InstanceInputs);
        await File.WriteAllTextAsync(saveFile, content);
        ExportFilePath = saveFile;
            
        await App.Current.MainPage.DisplayAlert("通知", "InstanceInputs.setting成功","确认");
    }
}