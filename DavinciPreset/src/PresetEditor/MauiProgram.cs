using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PresetEditor.ViewModels;
using PresetEditor.Views;
using Syncfusion.Maui.Toolkit.Hosting;
using zoft.MauiExtensions.Controls;

namespace PresetEditor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options=>
                {
#if WINDOWS
                    options.SetShouldEnableSnackbarOnWindows(true);
#endif
                    options.SetPopupDefaults(new DefaultPopupSettings
                    {
                        // CanBeDismissedByTappingOutsideOfPopup = true,
                        // BackgroundColor = Colors.Orange,
                        // HorizontalOptions = LayoutOptions.End,
                        // VerticalOptions = LayoutOptions.Start,
                        Margin = 0,
                        Padding = 0
                    });
                    options.SetPopupOptionsDefaults(new DefaultPopupOptionsSettings
                    {
                        // CanBeDismissedByTappingOutsideOfPopup = true,
                        // OnTappingOutsideOfPopup = async () => await Toast.Make("Popup Dismissed").Show(CancellationToken.None),
                        // PageOverlayColor = Colors.Orange,
                        Shadow = null,
                        Shape = null
                    });
                })
                .ConfigureSyncfusionToolkit()
                .UseZoftAutoCompleteEntry()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
    				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<IPresetSettingSegment, PresetSettingSegment>();
            
            builder.Services.AddSingleton<PresetPickerPageModel>();
            builder.Services.AddSingleton<PresetNodeEditPopupViewModel>();
            builder.Services.AddSingleton<PresetNodeView>();
            builder.Services.AddSingleton<GroupNodeView>();

            builder.Services.AddSingleton<AppShell, AppShellPageModel>();
            builder.Services.AddSingleton<FlyoutView>();
            builder.Services.AddSingleton<ProjectTheme>();
            builder.Services.AddSingleton<DashboardPage,DashboardPageModel>();
            builder.Services.AddSingleton<PublishInputsPage,PublishInputsPageModel>();
            builder.Services.AddSingleton<GroupSourcesPage,GroupSourcesPageModel>();
            builder.Services.AddSingleton<PlansPage>();

            builder.Services.AddTransientPopup<PresetNodeEditPopupView, PresetNodeEditPopupViewModel>();
            builder.Services.AddTransientPopup<GroupNodeEditPopupView, GroupNodeEditPopupViewModel>();

            return builder.Build();
        }
    }
}
