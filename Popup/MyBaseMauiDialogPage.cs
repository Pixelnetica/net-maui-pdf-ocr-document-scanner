using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsDemoApplication.Popup
{
    internal class MyBaseDialogPage: CommunityToolkit.Maui.Views.Popup
    {
        protected StackLayout MainLayout = new StackLayout();

        public static Color MenuFontColor = Colors.Black;
        public static Color MenuBackColor = Colors.White; //Color.FromRgba(50,50,50,255);  //Colors.White;
        public static double MenuFontSize = 16;// Device.GetNamedSize(NamedSize.Header, new Label());
        public static double MenuDescriptionFontSize = MenuFontSize;

        public static double MenuItemHeight = 48;
        public static double MenuDivSize = 4;
        public static double MenuPadding = 10;

        public MyBaseDialogPage(double top) :base()
        {

            //var myLabel = new Label();
            //MenuDescriptionFontSize=MenuFontSize =  Device.GetNamedSize(NamedSize.Default, myLabel);
#if _FORMS_
            //MenuDescriptionFontSize = 14;
            //MenuFontSize = 14;
#else
            this.SetAppThemeColor(SolidColorBrush.ColorProperty, Colors.Black, Colors.White);
            if (MyBaseDialogPage.MenuFontColor != null) MenuFontColor = Popup.MyBaseDialogPage.MenuFontColor;
            
            //this.SetAppThemeColor(SolidColorBrush.ColorProperty, Colors.White, Color.FromRgba(50, 50, 50, 255));
            if (MyBaseDialogPage.MenuBackColor != null) MenuBackColor = Popup.MyBaseDialogPage.MenuBackColor;
            
            //base.Color = MenuBackColor;
#endif


            var m = new ScrollView() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.Start };
#if _FORMS_
            m.Margin = new Thickness(0, top, 0, 0);
#endif

            MainLayout.Spacing = MenuDivSize;
            MainLayout.VerticalOptions = LayoutOptions.Fill;
            MainLayout.HorizontalOptions = LayoutOptions.Fill;
            //MainLayout.BackgroundColor = Colors.Red;

            //maui <12
            //base.VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            base.VerticalOptions = LayoutOptions.Center;
            //base.HorizontalOptions = LayoutOptions.Fill;
            base.Padding = new Thickness();
            
            m.Content = MainLayout;// f;
            MainLayout.Spacing = 0;
            

            base.Content = m;
        }

       
        public void ShowDialog()
        {
            MainThread.BeginInvokeOnMainThread(() =>Application.Current.MainPage.ShowPopup(this));
            //Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(this);
        }

        public void CloseDialog()
        {
            base.CloseAsync();
            //try
            //{
            //    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            //}
            //catch { };

            //this.Hi
        }

    }
}
