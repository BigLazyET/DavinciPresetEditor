using PresetEditor.Enums;

namespace PresetEditor.Views;

public partial class FlyoutView : ContentView
{
    private readonly AppShellPageModel _pageModel;
    
    public FlyoutView()
    {
        InitializeComponent();
        BindingContext = _pageModel = App.Current.ServiceProvider.GetRequiredService<AppShellPageModel>();
    }
    
    public async Task CustomJump(HomeMenuItemType menuItemType)
    {
        var selectedItem = _pageModel.HomeMenuItems.First(item => item.MenuType == menuItemType);
        await _pageModel.SelectionChanged(selectedItem);
    }

    private void PopupThemeSwitchView(object? sender, TappedEventArgs e)
    {
        Application.Current!.UserAppTheme = Application.Current!.UserAppTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        if (Application.Current!.UserAppTheme == AppTheme.Dark)
            ModeImg.Source = ModeImg.Source = (ImageSource)Application.Current!.Resources["IconLight"];
        else
            ModeImg.Source = ModeImg.Source = (ImageSource)Application.Current!.Resources["IconDark"];
    }
    
    private async void EmailMe(object? sender, TappedEventArgs e)
    {
        try
        {
            var result =await App.Current.MainPage.DisplayAlert("提醒", $"通过邮箱联系我❤️{Environment.NewLine}xjl505302554@outlook.com", "好的", "暂不");
            if (!result) return;

            var message = new EmailMessage
            {
                Subject = "Hello from Davinci Preset Editor",
                Body = "I have a good Suggestion or Question!.",
                To = new List<string> { "xjl505302554@outlook.com" },
            };

            await Email.ComposeAsync(message);
        }
        catch
        {
            await App.Current.MainPage.DisplayAlert("提醒", $"邮件应用调用失败，如有疑问或建议{Environment.NewLine}通过xjl505302554@outlook.com联系我❤️", "好的");
        }
        
    }
    
    private async void NavigateToProjectGithub(object? sender, TappedEventArgs e)
    {
        var uri = new Uri("https://github.com/BigLazyET");
        await Launcher.OpenAsync(uri);
    }
    
    private async void BuMeCoffee(object? sender, TappedEventArgs e)
    {
        await App.Current.MainPage.DisplayAlert("提醒", "Buy Me A Coffee","确认");
    }
}