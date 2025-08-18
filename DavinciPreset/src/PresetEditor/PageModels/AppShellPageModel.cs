using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresetEditor.Enums;
using PresetEditor.Localizations;
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
            new HomeMenuItem { MenuType = HomeMenuItemType.Dashboard, Title = LocalizationResourceManager.Instance["Dashboard"].ToString(), Icon = "dashboard.png", Route = "dashboard", BackColor = Color.FromArgb("#007DE6") },
            new HomeMenuItem { MenuType = HomeMenuItemType.PublishInputs, Title = LocalizationResourceManager.Instance["PublishedParas"].ToString(), Icon = "publish.png", Route = "publish_inputs" },
            new HomeMenuItem { MenuType = HomeMenuItemType.GroupSources, Title = LocalizationResourceManager.Instance["GroupSource"].ToString(), Icon = "group.png", Route = "group_sources" },
            new HomeMenuItem { MenuType = HomeMenuItemType.Plans, Title = LocalizationResourceManager.Instance["Plans"].ToString(), Icon = "plans.png", Route = "plans" },
        ];
        _preSelectedItem = HomeMenuItems[0];
        
        LocalizationResourceManager.Instance.OnCultureChanged += OnCultureChanged;
    }

    private void OnCultureChanged()
    {
        foreach (var homeMenuItem in HomeMenuItems)
        {
            _ = homeMenuItem.MenuType switch
            {
                HomeMenuItemType.Dashboard => homeMenuItem.Title =
                    LocalizationResourceManager.Instance["Dashboard"].ToString(),
                HomeMenuItemType.PublishInputs => homeMenuItem.Title =
                    LocalizationResourceManager.Instance["PublishedParas"].ToString(),
                HomeMenuItemType.GroupSources => homeMenuItem.Title =
                    LocalizationResourceManager.Instance["GroupSource"].ToString(),
                HomeMenuItemType.Plans => homeMenuItem.Title = LocalizationResourceManager.Instance["Plans"].ToString(),
            };
        }
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