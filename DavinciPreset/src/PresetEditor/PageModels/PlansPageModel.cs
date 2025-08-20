using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Models;

namespace PresetEditor.PageModels;

public partial class PlansPageModel : ObservableObject
{
    private readonly DashboardPageModel _dashboardPageModel;
    private readonly ICombineConfigService _combineConfigService;
    private readonly IPresetSettingSegment _presetSettingSegment;
    private readonly GroupSourcesPageModel _groupSourcesPageModel;
    private readonly PublishInputsPageModel _publishInputsPageModel;
    
    public PlansPageModel(DashboardPageModel dashboardPageModel, ICombineConfigService combineConfigService, IPresetSettingSegment presetSettingSegment, 
        GroupSourcesPageModel groupSourcesPageModel, PublishInputsPageModel publishInputsPageModel)
    {
        _dashboardPageModel = dashboardPageModel;
        _combineConfigService = combineConfigService;
        _presetSettingSegment = presetSettingSegment;
        _groupSourcesPageModel = groupSourcesPageModel;
        _publishInputsPageModel = publishInputsPageModel;
    }

    [RelayCommand]
    private async Task ExportGlobalConfig()
    {
        var originial = _dashboardPageModel.SettingContent;
        if (string.IsNullOrWhiteSpace(originial))
        {
            // TODO
            return;
        }

        var instanceInputsContent = await _publishInputsPageModel.InstanceInputsContent();
        var originalInstanceInputs = _combineConfigService.ReplaceBlockByKeyword(originial, "Inputs = ordered()", instanceInputsContent);
        // 写入GroupInputs
        var groupSegment = $"{_groupSourcesPageModel.GroupSourceOp} = {_groupSourcesPageModel.GroupSourceOpType}";
        var groupInputsContent = await _groupSourcesPageModel.GroupInputsContent();
        var finalContent = _combineConfigService.ReplaceGroupBlockByKeyword(originalInstanceInputs, groupSegment, groupInputsContent);
        
        var pickResult = await FolderPicker.Default.PickAsync();
        if (pickResult?.Folder == null || string.IsNullOrWhiteSpace(pickResult.Folder.Path)) return;

        var saveFile = Path.Combine(pickResult.Folder.Path, "global.setting");
        await File.WriteAllTextAsync(saveFile, finalContent);
    }
}