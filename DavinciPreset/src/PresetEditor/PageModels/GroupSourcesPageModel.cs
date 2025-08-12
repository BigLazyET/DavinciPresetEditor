using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Models;
using PresetEditor.ViewModels;

namespace PresetEditor.PageModels;

public partial class GroupSourcesPageModel : ObservableObject
{
    private readonly IPopupService _popupService;
    private readonly IPresetSettingSegment _presetSettingSegment;
    
    [ObservableProperty] private string _groupSourceOp;
    [ObservableProperty] private string _groupSourceOpType;
    [ObservableProperty] private ObservableCollection<GroupInput> _groupInputs = [];
    [ObservableProperty] private GroupInput? _selectedGroupInput;
    
    [ObservableProperty] private string _exportFilePath;
    
    public GroupSourcesPageModel(IPopupService popupService, IPresetSettingSegment presetSettingSegment)
    {
        _popupService = popupService;
        _presetSettingSegment = presetSettingSegment;
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
            
        var dashboardPageModel = App.Current.ServiceProvider.GetRequiredService<DashboardPageModel>();
        var settingContent = dashboardPageModel.SettingContent;
        if (string.IsNullOrEmpty(settingContent)) return;
        
        var groupSegment = $"{GroupSourceOp} = {GroupSourceOpType}";
        var groupInputs = _presetSettingSegment.GetGroupInputs(settingContent, groupSegment);
        if (groupInputs == null) return;
        foreach (var groupInput in groupInputs)
            GroupInputs.Add(groupInput);
    }
    
    [RelayCommand]
    private async Task AddGroupInput()
    {
        if (string.IsNullOrWhiteSpace(GroupSourceOp))
        {
            await App.Current.MainPage.DisplayAlert("提醒", "分组节点名不能为空","确认");
            return;
        }
            
        if (string.IsNullOrWhiteSpace(GroupSourceOpType))
        {
            await App.Current.MainPage.DisplayAlert("提醒", "分组节点类型不能为空","确认");
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
            await App.Current.MainPage.DisplayAlert("提醒", "分组名重复，添加失败","确认");
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
    private async Task ExportGroupInputs()
    {
        if (GroupInputs.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert("确认", "⚠️当前分组节点没有任何配置项","确认");
            return;
        }
            
        var pickResult = await FolderPicker.Default.PickAsync();
        if (pickResult?.Folder == null || string.IsNullOrWhiteSpace(pickResult.Folder.Path)) return;

        var saveFile = Path.Combine(pickResult.Folder.Path, "GroupInputs.setting");
        var content = _presetSettingSegment.OrderedGroups2Text(GroupInputs);
        await File.WriteAllTextAsync(saveFile, content);
        ExportFilePath = saveFile;
            
        await App.Current.MainPage.DisplayAlert("通知", "生成GroupInputs.setting成功","确认");
    }
}