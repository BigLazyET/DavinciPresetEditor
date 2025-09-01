using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
using PresetEditor.Localizations;
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
    [ObservableProperty] private ObservableCollection<string> _filteredInputNames = [];

    [ObservableProperty] private string _exportFilePath;

    [ObservableProperty] private bool _isLoading;
    
    private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
    private string Confirm => LocalizationResourceManager.Instance["Confirm"].ToString();
    private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();

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
            var dashboardPageModel = App.Current.ServiceProvider.GetRequiredService<DashboardPageModel>();
            var settingContent = dashboardPageModel.SettingContent;
            if (string.IsNullOrEmpty(settingContent))
            {
                await App.Current.MainPage!.DisplayAlert(Remind, LocalizationResourceManager.Instance["TemplateFileRemind"].ToString(),Confirm);
                return;
            }
            
            var result = await App.Current.MainPage!.DisplayAlert(Remind,LocalizationResourceManager.Instance["LoadRemind"].ToString(), Confirm,Cancel);
            if (!result) return;
            
            IsLoading = true;

            var instanceInputRaws = await Task.Run(() =>
                _presetSettingSegment.GetOrderedInstanceInputs(settingContent)
            );

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
    private void DeleteMoveInputName(InstanceInput input)
    {
        SelectedInstanceInputs.Remove(input);
    }
    
    [RelayCommand]
    private async Task Move()
    {
        if (string.IsNullOrWhiteSpace(BaseInputName))
        {
            await App.Current.MainPage!.DisplayAlert(Remind, LocalizationResourceManager.Instance["ReorderTargetRemind"].ToString(),Confirm);
            return;
        }

        if (SelectedInstanceInputs.Count == 0)
        {
            await App.Current.MainPage!.DisplayAlert(Remind, LocalizationResourceManager.Instance["ReOrderRemind"].ToString(),Confirm);
            return;
        }

        var baseIndex = InstanceInputs.IndexOf(InstanceInputs.First(i => i.InputName == BaseInputName));
        foreach (var selectedInput in SelectedInstanceInputs.Reverse())
        {
            var moveInput = selectedInput as InstanceInput;
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
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["SelectItemRemind"].ToString(),Confirm);
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
        
        resultInstanceInput.PropertyListChanged();
        
    }

    [RelayCommand]
    private async Task DeleteInstanceInput()
    {
        if (SelectedInstanceInputs.Count is 0)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["SelectItemsRemind"].ToString(),Confirm);
            return;
        }
        
        var res =  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["DeleteRemind"].ToString(),Confirm, Cancel);
        if (!res) return;

        var toRemove = SelectedInstanceInputs.OfType<InstanceInput>().ToList();
        foreach (var instanceInput in toRemove)
        {
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
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupsEmptyRemind"].ToString(),Confirm);
            return;
        }

        if (string.IsNullOrWhiteSpace(groupSourceOp))
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupNodeEmptyRemind"].ToString(),Confirm);
            return;
        }
        
        var res=  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["SyncGroupsRemind"].ToString(),Confirm, Cancel);
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
        try
        {
            if (InstanceInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["ParasEmptyRemind"].ToString(), Confirm);
                return;
            }

            IsLoading = true;
            var content = await InstanceInputsContent();
            if (string.IsNullOrWhiteSpace(content)) return;
            IsLoading = false;

            var pickResult = await FolderPicker.Default.PickAsync();
            if (pickResult?.Folder == null || string.IsNullOrWhiteSpace(pickResult.Folder.Path)) return;
            var saveFile = Path.Combine(pickResult.Folder.Path, "InstanceInputs.setting");
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

    public async Task<string> InstanceInputsContent()
    {
        if (InstanceInputs.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["ParasEmptyRemind"].ToString(),Confirm);
            return string.Empty;
        }
        var content = _presetSettingSegment.OrderedInputs2Text(InstanceInputs);
        return content;
    }
}