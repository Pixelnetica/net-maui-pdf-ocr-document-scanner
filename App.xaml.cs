using XamarinFormsDemoApplication;

namespace MauiDemoApplication
{
    public partial class App : Application
    {
        public App()
        {
            //InitializeComponent();

            MainPage = new XamarinFormsDemoApplication.MainPage(null);
        }
    }
}