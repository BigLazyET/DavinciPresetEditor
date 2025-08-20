using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PresetEditor.PageModels;

public partial class DashboardPageModel : ObservableObject
{
    public string? SettingContent { get; private set; }
    
    [ObservableProperty] private string? _filePath;
    [ObservableProperty] private int _totalPublishInputs;
    [ObservableProperty] private int _totalGroupSources;
    [ObservableProperty] private int _totalPageCategories;
    
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
            SettingContent = await sr.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }
    }

    public void RefreshStatus()
    {
        var publishInputsPageModel = App.Current.ServiceProvider.GetRequiredService<PublishInputsPageModel>();
        var groupSourcesPageModel = App.Current.ServiceProvider.GetRequiredService<GroupSourcesPageModel>();
        
        TotalPublishInputs = publishInputsPageModel.InstanceInputs.Count;
        TotalGroupSources = groupSourcesPageModel.GroupInputs.Count;
        var pages = publishInputsPageModel.InstanceInputs.Select(i =>
            i.PropertyList.FirstOrDefault(p => p.Key == "Page")?.Value ?? string.Empty).Distinct();
        if (!publishInputsPageModel.InstanceInputs.Any())
        {
            TotalPageCategories = 0;
            return;
        }
        TotalPageCategories = pages.Count() is 0 or 1 ? 1 : pages.Count() - 1;
    }
}