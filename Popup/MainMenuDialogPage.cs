using ImageSdkWrapper;
using System;
using System.Collections.Generic;
using System.Text;


namespace XamarinFormsDemoApplication.Popup
{
    internal class MyMenuItem
    {
        public string Caption;
        public System.Action OnClick;

        public MyMenuItem(string caption, System.Action ev)
        {
            Caption = caption;
            OnClick = ev;
        }
    }

    internal class MainMenuDialogPage : MyBaseDialogPage
    {

        public MainMenuDialogPage( List<MyMenuItem> menu,double top):base(top)
        {
            
            foreach (var it in menu)
            {
                var r = new MyLabel(false,it.Caption);
                
                r.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        this.Closed += (object sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e) =>
                        {
                            MainThread.BeginInvokeOnMainThread(it.OnClick);
                        };
                        base.CloseDialog();
                    })
                });
                MainLayout.Children.Add(r);
            }

        }

        
    }
}
