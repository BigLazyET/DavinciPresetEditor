using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Localizations;

namespace PresetEditor.PageModels;

public partial class PlansPageModel : ObservableObject
{
    private readonly DashboardPageModel _dashboardPageModel;
    private readonly ICombineConfigService _combineConfigService;
    private readonly GroupSourcesPageModel _groupSourcesPageModel;
    private readonly PublishInputsPageModel _publishInputsPageModel;
    
    [ObservableProperty] private bool _isLoading;
    
    private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
    private string Confirm => LocalizationResourceManager.Instance["Confirm"].ToString();
    private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();
    
    public PlansPageModel(DashboardPageModel dashboardPageModel, ICombineConfigService combineConfigService,
        GroupSourcesPageModel groupSourcesPageModel, PublishInputsPageModel publishInputsPageModel)
    {
        _dashboardPageModel = dashboardPageModel;
        _combineConfigService = combineConfigService;
        _groupSourcesPageModel = groupSourcesPageModel;
        _publishInputsPageModel = publishInputsPageModel;
    }

    [RelayCommand]
    private async Task ExportGlobalConfig()
    {
        try
        {
            var result = await App.Current.MainPage!.DisplayAlert(Remind,
                LocalizationResourceManager.Instance["PreviewRemind"].ToString(), Confirm, Cancel);
            if (!result) return;

            var settingContent = _dashboardPageModel.SettingContent;
            if (string.IsNullOrEmpty(settingContent))
            {
                await App.Current.MainPage!.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["TemplateFileRemind"].ToString(), Confirm);
                return;
            }

            if (_publishInputsPageModel.InstanceInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["ParasEmptyRemind"].ToString(), Confirm);
                return;
            }

            if (_groupSourcesPageModel.GroupInputs.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert(Remind,
                    LocalizationResourceManager.Instance["GroupsEmptyRemind"].ToString(), Confirm);
                return;
            }

            IsLoading = true;
            var instanceInputsContent = await _publishInputsPageModel.InstanceInputsContent();
            var updatedInstanceInputsContent =
                _combineConfigService.ReplaceBlockByKeyword(settingContent, "Inputs = ordered()",
                    instanceInputsContent);
            var groupSegment = $"{_groupSourcesPageModel.GroupSourceOp} = {_groupSourcesPageModel.GroupSourceOpType}";
            var groupInputsContent = await _groupSourcesPageModel.GroupInputsContent();
            if (string.IsNullOrWhiteSpace(updatedInstanceInputsContent)) return;
            var finalContent =
                _combineConfigService.ReplaceGroupBlockByKeyword(updatedInstanceInputsContent!, groupSegment,
                    groupInputsContent);
            if (string.IsNullOrWhiteSpace(finalContent)) return;
            // var dslFormattedContent = DSLFormatter.Format(finalContent!);
            IsLoading = false;
            
            var pickResult = await FolderPicker.Default.PickAsync();
            if (pickResult?.Folder == null || string.IsNullOrWhiteSpace(pickResult.Folder.Path)) return;

            var saveFile = Path.Combine(pickResult.Folder.Path, "Global.setting");
            await File.WriteAllTextAsync(saveFile, finalContent);

            await App.Current.MainPage.DisplayAlert(Remind,
                LocalizationResourceManager.Instance["ExportFilePathRemind"] + Environment.NewLine + saveFile, Confirm);
        }
        finally
        {
            IsLoading = false;
        }
    }
}