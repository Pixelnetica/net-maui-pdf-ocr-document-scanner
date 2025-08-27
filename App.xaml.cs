using XamarinFormsDemoApplication;

namespace MauiDemoApplication
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            UserAppTheme = AppTheme.Light;

            MainPage = new XamarinFormsDemoApplication.MainPage(null);
        }
    }
}