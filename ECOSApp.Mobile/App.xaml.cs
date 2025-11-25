namespace ECOSApp.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            // The call to MainPage = ... is removed from the constructor.
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 1. Create the root Page, wrapped in a NavigationPage (as you had before).
            var rootPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromRgb(102, 126, 234),
                BarTextColor = Colors.White
            };

            // 2. Call the base method to create the Window object.
            var window = base.CreateWindow(activationState);

            // 3. Set the root page on the Window object.
            window.Page = rootPage;

            // 4. Return the configured Window.
            return window;
        }
    }
}