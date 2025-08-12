using PresetEditor.Enums;

namespace PresetEditor.Models;

public class HomeMenuItem : MenuItem
{
    private string _route;
    private HomeMenuItemType _menuType;

    public string Route
    {
        get => _route;
        set => SetProperty(ref _route, value);
    }
    
    public HomeMenuItemType MenuType
    {
        get => _menuType;
        set => SetProperty(ref _menuType, value);
    }
}