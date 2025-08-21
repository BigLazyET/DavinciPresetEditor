using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Localizations;
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
    
    [ObservableProperty] private bool _isLoading;

    public IList<GroupInput> RawGroupInputs { get; private set; } = [];
    
    private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
    private string Confirm => LocalizationResourceManager.Instance["Confirm"].ToString();
    private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();
    
    public GroupSourcesPageModel(IPopupService popupService, IPresetSettingSegment presetSettingSegment)
    {
        _popupService = popupService;
        _presetSettingSegment = presetSettingSegment;
    }
    
    [RelayCommand]
    private async Task GroupCollect()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(GroupSourceOp))
            {
                await App.Current.MainPage.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["GroupNodeEmptyRemind"].ToString(), Confirm);
                return;
            }

            if (string.IsNullOrWhiteSpace(GroupSourceOpType))
            {
                await App.Current.MainPage.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["GroupNodeTypeEmptyRemind"].ToString(), Confirm);
                return;
            }

            var dashboardPageModel = App.Current.ServiceProvider.GetRequiredService<DashboardPageModel>();
            var settingContent = dashboardPageModel.SettingContent;
            if (string.IsNullOrEmpty(settingContent)) return;

            IsLoading = true;

            var groupSegment = $"{GroupSourceOp} = {GroupSourceOpType}";
            var groupInputs = await Task.Run(()=> _presetSettingSegment.GetGroupInputs(settingContent, groupSegment));
            if (groupInputs == null || !groupInputs.Any())
            {
                RawGroupInputs.Clear();
                GroupInputs.Clear();
                return;
            }

            RawGroupInputs.Clear();
            GroupInputs.Clear();
            RawGroupInputs = groupInputs.ToList();
            foreach (var groupInput in groupInputs)
            {
                var group = groupInput.PropertyList.FirstOrDefault(p => p.Key == "LBLC_NumInputs");
                if (group == null) continue;
                GroupInputs.Add(groupInput);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task AddGroupInput()
    {
        if (string.IsNullOrWhiteSpace(GroupSourceOp))
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupNodeEmptyRemind"].ToString(),Confirm);
            return;
        }
            
        if (string.IsNullOrWhiteSpace(GroupSourceOpType))
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupNodeTypeEmptyRemind"].ToString(),Confirm);
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
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupDumplicateRemind"].ToString(),Confirm);
            return;
        }
        GroupInputs.Add(resultGroupInput);
    }
        
    [RelayCommand]
    private async Task ModifyGroupInput()
    {
        if (SelectedGroupInput == null)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["SelectItemRemind"].ToString(),Confirm);
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
        
        resultGroupInput.PropertyListChanged();
    }
        
    [RelayCommand]
    private async Task DeleteGroupInput()
    {
        if (SelectedGroupInput == null)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["SelectItemRemind"].ToString(),Confirm);
            return;
        }
            
        var res=  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["DeleteRemind"].ToString(),Confirm, Cancel);
        if (!res) return;
        GroupInputs.Remove(SelectedGroupInput);
    }
    
    [RelayCommand]
    private async Task ExportGroupInputs()
    {
        try
        {
            if (GroupInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["GroupsEmptyRemind"].ToString(), Confirm);
                return;
            }
            
            IsLoading = true;
            var content = await GroupInputsContent();
            if (string.IsNullOrWhiteSpace(content)) return;
            IsLoading = false;

            var pickResult = await FolderPicker.Default.PickAsync();
            if (pickResult?.Folder == null || string.IsNullOrWhiteSpace(pickResult.Folder.Path)) return;
            var saveFile = Path.Combine(pickResult.Folder.Path, "GroupInputs.setting");
            await File.WriteAllTextAsync(saveFile, content);
            ExportFilePath = saveFile;

            await App.Current.MainPage.DisplayAlert(Remind,
                LocalizationResourceManager.Instance["ExportFilePathRemind"].ToString(), Confirm);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<string> GroupInputsContent()
    {
        if (GroupInputs.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupsEmptyRemind"].ToString(),Confirm);
            return string.Empty;
        }
        
        var rawExcepts = RawGroupInputs.Where(r => r.PropertyList.Any(p => p.Key == "LBLC_NumInputs") == false).ToList();
        rawExcepts.AddRange(GroupInputs);
        var content = _presetSettingSegment.OrderedGroups2Text(rawExcepts);
        return content;
    }
}