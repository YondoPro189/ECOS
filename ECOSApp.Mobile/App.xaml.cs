namespace ECOSApp.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromRgb(102, 126, 234),
                BarTextColor = Colors.White
            };
        }
    }
}