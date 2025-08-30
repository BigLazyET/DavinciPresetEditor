using System.Globalization;
using PresetEditor.Enums;
using PresetEditor.Localizations;

namespace PresetEditor.Views;

public partial class FlyoutView : ContentView
{
    private readonly AppShellPageModel _pageModel;
    
    public FlyoutView()
    {
        InitializeComponent();
        BindingContext = _pageModel = App.Current.ServiceProvider.GetRequiredService<AppShellPageModel>();
        
        LanguageImg.Source = App.Current.IsChineseLanguage() ? "en.png" : "zh.png";
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
            var result =await App.Current.MainPage.DisplayAlert("提醒", $"Email me {Environment.NewLine}xjl505302554@outlook.com", "好的", "暂不");
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
            await App.Current.MainPage.DisplayAlert("提醒", $"邮件应用调用失败，如有疑问或建议{Environment.NewLine}通过xjl505302554@outlook.com联系我", "好的");
        }
        
    }
    
    private async void NavigateToProjectGithub(object? sender, TappedEventArgs e)
    {
        var uri = new Uri("https://github.com/BigLazyET/DavinciPresetEditor");
        await Launcher.OpenAsync(uri);
    }
    
    private async void BuMeCoffee(object? sender, TappedEventArgs e)
    {
        await App.Current.MainPage.DisplayAlert("提醒", "Buy Me A Coffee","确认");
        var uri = new Uri("https://ko-fi.com/biglazyet");
        await Launcher.OpenAsync(uri);
    }
    
    private void SwitchLanguage(object? sender, TappedEventArgs e)
    {
        var isImageZhPng = IsImageZhPng(LanguageImg);
        var languageImg = isImageZhPng ? "en.png" : "zh.png";
        LanguageImg.Source = languageImg;
        LocalizationResourceManager.Instance.Culture = languageImg == "en.png" ? new CultureInfo("zh-CN") : new CultureInfo("en-US");
    }
    
    private bool IsImageZhPng(Image imageControl)
    {
        if (imageControl?.Source is not FileImageSource fileSource) return false;
        var fileName = System.IO.Path.GetFileName(fileSource.File);
        return fileName.Equals("zh.png", StringComparison.OrdinalIgnoreCase);
    }
}