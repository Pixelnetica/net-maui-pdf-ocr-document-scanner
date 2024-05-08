using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsDemoApplication.Popup
{
    internal class AboutDialogPage : MyBaseDialogPage
    {
        public AboutDialogPage():base(0)
        {
            

            string appname;
            string pkgName = AppInfo.PackageName;
            string hash = "";
            try
            {
                hash = ImageSdkWrapper.Main.GitHashValue.Length > 7 ? ImageSdkWrapper.Main.GitHashValue.Substring(0, 7) : ImageSdkWrapper.Main.GitHashValue;
            }
            catch { };
            string msg = pkgName+"\n"+string.Format("Version {0} ({1})", MyTools.GetAppVersion(out appname), hash);

            
            StackLayout hdr = new StackLayout() { Orientation = StackOrientation.Horizontal };
            MainLayout.Children.Add(hdr);
                        
            Image img = new Image() { Source = ImageSource.FromFile("icon.png") };
            img.VerticalOptions = LayoutOptions.FillAndExpand;
            img.HeightRequest = 25;
            hdr.Children.Add(img);

            var headerTxt = new MyLabel(false, appname );
            //header.FontSize= header.FontSize*1.5
            headerTxt.FontAttributes = FontAttributes.Bold;
            headerTxt.VerticalOptions= LayoutOptions.Center;
            hdr.Children.Add(headerTxt);
                        
            MainLayout.Children.Add(new MyLabel(true, msg));

            {
                StackLayout s = new StackLayout() { Orientation = StackOrientation.Horizontal };
                s.Children.Add(new MyLabel(true,"Powered by "));
                s.Spacing = 0;
                                
                var url = new MyLabel(true,"Document Scaning SDK");
                s.Children.Add(url);
                url.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { OnDocumentScaningSdkUrl(); }) });
                url.TextDecorations = TextDecorations.Underline;
                url.TextColor = Colors.Blue;
                MainLayout.Children.Add(s);
                MainLayout.Children.Add(new MyLabel(true,GetLibVersion()));
            }

            MainLayout.Children.Add(new MyLabel(true,"©Pixelnetica. All rights reserved."));
            MainLayout.Children.Add(new MyLabel(true, ""));
            {
                var url = new MyLabel(true, "DSSDK Support");
                url.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { OnDSSDKkUrl(); }) });
                url.TextDecorations = TextDecorations.Underline;
                url.TextColor = Colors.Blue;
                MainLayout.Children.Add(url);
            }

            var ok = new Button() { Text = "OK", HorizontalOptions = LayoutOptions.End, Margin = new Thickness(MenuPadding) };
            ok.Clicked += OnOK;
            MainLayout.Children.Add(ok);

        }

        string GetLibVersion()
        {
            try
            {
                var a = typeof(ImageSdkWrapper.Maui.AppBuilderExtensions).Assembly;
                var tmp = a.FullName.Split(", ");
                foreach (var t in tmp)
                {
                    var parts = t.Split('=');
                    if (parts.Length == 2 && parts[0].Trim().ToLower() == "version")
                    {
                        return parts[1].Trim();
                    }
                }
            }
            catch { }
            return "";
        }

        void OnOK(object o,EventArgs e)        
        {
            base.CloseDialog();
            

        }

        void OnDocumentScaningSdkUrl()
        {
            Browser.OpenAsync("https://www.pixelnetica.com/products/document-scanning-sdk/document-scanner-sdk.html?utm_source=EasyScan&utm_medium=src-xamarin_forms&utm_campaign=scr-about&utm_content=dssdk-overview");
        }

        void OnDSSDKkUrl()
        {
            Browser.OpenAsync("https://www.pixelnetica.com/products/document-scanning-sdk/sdk-support.html?utm_source=EasyScan&utm_medium=src-xamarin_forms&utm_campaign=scr-about&utm_content=dssdk-support");
        }

    }
}
