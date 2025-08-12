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
    }
}