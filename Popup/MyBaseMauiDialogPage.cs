using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsDemoApplication.Popup
{
    internal class MyBaseDialogPage: Grid
    {
        protected StackLayout MainLayout = new StackLayout();

        public static Color MenuFontColor = Colors.Black;
        public static Color MenuBackColor = Colors.White; //Color.FromRgba(50,50,50,255);  //Colors.White;
        public static double MenuFontSize = 16;// Device.GetNamedSize(NamedSize.Header, new Label());
        public static double MenuDescriptionFontSize = MenuFontSize;

        public static double MenuItemHeight = 48;
        public static double MenuDivSize = 4;
        public static double MenuPadding = 10;

        MainPage _MainPage;
        
        public MyBaseDialogPage(MainPage p,double top) :base()
        {
            _MainPage = p;


            this.BackgroundColor = new Color(0, 0, 0, 150);
            this.InputTransparent = false;
            this.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(CloseDialog) });

            //this.VerticalOptions=this.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            this.VerticalOptions = this.HorizontalOptions = LayoutOptions.FillAndExpand;

            var myLabel = new Label();
            MenuDescriptionFontSize = MenuFontSize = 16;// Device.GetNamedSize(NamedSize.Default, myLabel);
#if _FORMS_
            //MenuDescriptionFontSize = 14;
            //MenuFontSize = 14;
#else
            //this.SetAppThemeColor(ColorProperty, Colors.Black, Colors.White);
            //MenuFontColor = Color;

            //this.SetAppThemeColor(ColorProperty, Colors.White, Color.FromRgba(50, 50, 50, 255));
            //MenuBackColor = Color;


            //base.Color = MenuBackColor;
#endif

            var m = new ScrollView();
            m.VerticalOptions = m.HorizontalOptions = top > 0 ? LayoutOptions.Start : LayoutOptions.Center;
            //m.HorizontalOptions = LayoutOptions.Center;
            
            //m.SetAppThemeColor(ColorProperty, Colors.Black, Colors.White);
            //MenuFontColor = m.Tex

            m.SetAppThemeColor(ScrollView.BackgroundColorProperty, Colors.White, Color.FromRgba(50, 50, 50, 255));
            MenuBackColor = m.BackgroundColor;

#if _FORMS_
            m.Margin = new Thickness(0, top, 0, 0);
#endif

            MainLayout.Spacing = MenuDivSize;
            MainLayout.VerticalOptions = LayoutOptions.Start;
            MainLayout.HorizontalOptions = LayoutOptions.Start;

            m.Content = MainLayout;// f;
            MainLayout.Spacing = 0;
            m.Margin = new Thickness(0, top, 0, 0);
            this.Children.Add(m);

            p.SetPopupLayout(this);
        }

       
        public void ShowDialog(MainPage p)
        {
            
            //MainThread.BeginInvokeOnMainThread(() =>p.ShowPopupAsync(this));
            
        }

        public void SetVerticalLayoutOptions(LayoutOptions layoutOptions)
        {
            ((View)this.Children[0]).VerticalOptions = layoutOptions;
        }

        public void CloseDialog()
        {
            _MainPage.SetPopupLayout(null);
            //Close();
            //try
            //{
            //    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            //}
            //catch { };

            //this.Hi
        }

    }
}
