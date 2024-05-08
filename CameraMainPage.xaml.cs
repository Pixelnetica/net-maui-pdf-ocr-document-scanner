using ImageSdkWrapper;
using ImageSdkWrapper.Maui;
using ImageSdkWrapper.Maui.Camera;
using ImageSdkWrapper.Forms;
using ImageSdkWrapper.Forms.Camera;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
//using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration;



namespace XamarinFormsDemoApplication
{
    public partial class CameraMainPage : ContentPage
    {
        PxlScannerView _PxlScannerView;

        public MetaImage OldMetaImage;

        public Thickness SafeAreaPadding { get => _PxlScannerView.SafeAreaPadding; set { _PxlScannerView.SafeAreaPadding = value; } }
        public CameraMainPage()
        {
            On<iOS>().SetUseSafeArea(false);

            _PxlScannerView = new PxlScannerView(); ;
            _PxlScannerView.VerticalOptions = LayoutOptions.FillAndExpand;
            _PxlScannerView.HorizontalOptions = LayoutOptions.FillAndExpand;
            _PxlScannerView.IgnoreSafeArea=true;

            _PxlScannerView.PictureReceiver = MyPictureReceiver;
            _PxlScannerView.OnReadyForStart += OnReadyForStart;
            _PxlScannerView.CloseClicked += OnCloseClicked;

            //PxlScannerView.Msg.shot_busy = "Camera busy";
            //PxlScannerView.Msg.shot_not_stable = "Camera is shaking";
            //PxlScannerView.Msg.looking_for_document = "Looking for document";
            //PxlScannerView.Msg.small_area = "Move Camera closer to document";
            //PxlScannerView.Msg.distorted = "Hold camera parallel to document";
            //PxlScannerView.Msg.unstable = "Don\'t move camera";
                        

            this.Content = _PxlScannerView;

         }

        protected override void OnDisappearing()
        {
            _PxlScannerView.Dispose();
            base.OnDisappearing();
        }

        void OnReadyForStart(object sender, System.EventArgs e)        
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await CheckPermissions();
                if (!_PxlScannerView.StartCamera())
                {
                    MyTools.MessageBox("Start camera error!");
                }
            });            
        }

        protected override bool OnBackButtonPressed()
        {            
            Task.Run(() =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Microsoft.Maui.Controls.Application.Current.MainPage = new MainPage(OldMetaImage);
                });
            });
            return true;

        }

        public async Task CheckPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    MyTools.MessageBox("No permissions for Camera");
                }
            }
        }

        void OnCloseClicked(object sender,EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Microsoft.Maui.Controls.Application.Current.MainPage = new MainPage(OldMetaImage);
            });
        }

        void MyPictureReceiver(MetaImage img, string errorTextOrNull)
        {
#if DEBUG
            Console.WriteLine(String.Format("time:Receive picture {0}ms", (int)(DateTime.UtcNow - _PxlScannerView.DebugTakePictureUtcTime).TotalMilliseconds));
#endif   

            if (img == null)
            {
                _PxlScannerView.Restart();
                MyTools.MessageBox(errorTextOrNull);
            }
            else
            {

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Microsoft.Maui.Controls.Application.Current.MainPage = new MainPage(img);
#if DEBUG
                    Console.WriteLine(String.Format("time:Afte shllpage {0}ms", (int)(DateTime.UtcNow - _PxlScannerView.DebugTakePictureUtcTime).TotalMilliseconds));
#endif
                }
                );
            }
        }

    }
}
