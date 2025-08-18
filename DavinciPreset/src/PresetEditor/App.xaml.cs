using System.Globalization;
using PresetEditor.Localizations;

namespace PresetEditor
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        
        public new static App Current => (App)Application.Current;
        
        public static AppShell AppShell { get; private set; }
        
        public App(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            
            InitializeComponent();

            LocalizationResourceManager.Instance.Culture = GetCultureInfo();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            AppShell = new AppShell();
            return new Window(AppShell);
        }
        
        public TView GetView<TView>() where TView : View
        {
            var view = ServiceProvider.GetRequiredService<TView>();
            return view;
        }
        
        public bool IsChineseLanguage()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName 
                .Equals("zh", StringComparison.OrdinalIgnoreCase);
        }

        private CultureInfo GetCultureInfo()
        {
            var cultureInfo = IsChineseLanguage() ? new CultureInfo("zh-CN") : new CultureInfo("en-US");
            return cultureInfo;
        }
    }
}