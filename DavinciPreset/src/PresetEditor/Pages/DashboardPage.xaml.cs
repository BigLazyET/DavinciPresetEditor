using PresetEditor.Enums;
using zoft.MauiExtensions.Controls;

namespace PresetEditor.Pages;

public partial class DashboardPage : ContentPage
{
    private DashboardPageModel _pageModel;
    
    public DashboardPage(DashboardPageModel pageModel)
    {
        InitializeComponent();
        
        BindingContext = _pageModel = pageModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        _pageModel.RefreshStatus();
    }

    private async void NavigateToPublishInputsPage(object? sender, EventArgs e)
    {
        await App.AppShell.CustomJump(HomeMenuItemType.PublishInputs);
    }
    
    private async void NavigateToGroupSourcesPage(object? sender, EventArgs e)
    {
        await App.AppShell.CustomJump(HomeMenuItemType.GroupSources);
    }
}