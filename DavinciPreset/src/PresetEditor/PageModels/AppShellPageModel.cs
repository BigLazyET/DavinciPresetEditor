using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Enums;
using PresetEditor.Models;

namespace PresetEditor.PageModels;

public partial class AppShellPageModel : ObservableObject
{
    private HomeMenuItem? _preSelectedItem;
    
    [ObservableProperty] private ObservableCollection<HomeMenuItem> _homeMenuItems;
    [ObservableProperty] private HomeMenuItem _profile = new() { MenuType = HomeMenuItemType.Profile, Route = "profile" };
    
    public AppShellPageModel()
    {
        HomeMenuItems =
        [
            new HomeMenuItem { MenuType = HomeMenuItemType.Dashboard, Title = "仪表盘", Icon = "dashboard.png", Route = "dashboard", BackColor = Color.FromArgb("#007DE6") },
            new HomeMenuItem { MenuType = HomeMenuItemType.PublishInputs, Title = "公开参数", Icon = "publish.png", Route = "publish_inputs" },
            new HomeMenuItem { MenuType = HomeMenuItemType.GroupSources, Title = "分组源", Icon = "group.png", Route = "group_sources" },
            new HomeMenuItem { MenuType = HomeMenuItemType.Plans, Title = "未来计划", Icon = "plans.png", Route = "plans" },
        ];
        _preSelectedItem = HomeMenuItems[0];
    }
    
    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task SelectionChanged(HomeMenuItem? item)
    {
        try
        {
            if (item == null && _preSelectedItem != null)
                _preSelectedItem.BackColor = Colors.Transparent;
            if (item == null) return;
            
            if (_preSelectedItem == item) return;
            if (_preSelectedItem != null)
                _preSelectedItem.BackColor = Colors.Transparent;
            item.BackColor = Color.FromArgb("#007DE6");
            _preSelectedItem = item;

            await Shell.Current.GoToAsync($"//{item.Route}");

        }
        catch (Exception ex)
        {
            
        }
    }
}