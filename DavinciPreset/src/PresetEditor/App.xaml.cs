namespace PresetEditor
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        
        public new static App Current => (App)Application.Current;
        
        public App(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        
        public TView GetView<TView>() where TView : View
        {
            var view = ServiceProvider.GetRequiredService<TView>();
            return view;
        }
    }
}