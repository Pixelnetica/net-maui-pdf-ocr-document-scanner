using ImageSdkWrapper;
using ImageSdkWrapper.Forms;
using ImageSdkWrapper.Maui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XamarinFormsDemoApplication.Popup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Diagnostics;
using MauiDemoApplication;

//using UIKit;


#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace XamarinFormsDemoApplication
{
    internal class MainPage : ContentPage
    {
        internal MainRecord _Record = new MainRecord();

        PxlCropImageView _CropImageFormsView;

        const double BUTTON_SIZE = 36;
                
        Layout _ToolBarLayout;
        StackLayout _PictureLayout = new StackLayout();

        static bool b_DebugFirstStart = true;

        public MainPage(MetaImage InitMetaImage)
        {
            StackLayout _MainLayout = new StackLayout();
            On<iOS>().SetUseSafeArea(false);
            //_MainLayout.IgnoreSafeArea = true;
            this.BackgroundColor = Colors.White;
            
            _PictureLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            _MainLayout.VerticalOptions = LayoutOptions.FillAndExpand;

            _PictureLayout.Spacing = 0;

            _CropImageFormsView = new PxlCropImageView() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };

            _CropImageFormsView.ActveChanged += _CropImageFormsView_ActveChanged;

            _PictureLayout.Children.Add(_CropImageFormsView);
                        
            _ToolBarLayout = _CropImageFormsView.TakeToolBar();
            _MainLayout.Children.Add(_ToolBarLayout);
            _ToolBarLayout.VerticalOptions = LayoutOptions.Start;
            _MainLayout.Children.Add(_PictureLayout);

            //_ToolBarLayout.IgnoreSafeArea = true;
            //_CropImageFormsView.IgnoreSafeArea = true;

            _CropImageFormsView.OnMenu += _CropImageFormsView_OnMenu;   

            //customization of CropImageFormsView
            //_CropImageFormsView.ToolBarHeight = BUTTON_SIZE;
            //_CropImageFormsView.ToolBarBackgroundColor = _CropImageFormsView.ToolBarBackgroundColor;
            //_CropImageFormsView.ToolBarPadding = new Thickness(16);
                        
            _CropImageFormsView.NonActiveEdgeColor = Color.FromUint(4278229452u);
            //_CropImageFormsView.EdgeActiveColor = Color.FromUint(4278246911u);
            _CropImageFormsView.EdgeMoveColor = Color.FromUint(1342234111u);
            _CropImageFormsView.EdgeInvalidColor = Color.FromUint(4294919236u);
            //_CropImageFormsView.CornerActiveColor = Color.FromUint(4281519410u);
            _CropImageFormsView.CornerMoveColor = Color.FromUint(2150812978u);
            _CropImageFormsView.CornerInvalidColor = Color.FromUint(4294919236u);
            _CropImageFormsView.ToolBarBackgroundColor = Color.FromUint(0xFFE0E0E0);


            _CropImageFormsView.TouchRadius = _CropImageFormsView.TouchRadius;

            //._CropImageFormsView.CornderRadius = _CropImageFormsView.CornderRadius;
            //_CropImageFormsView.EdgeThickness = _CropImageFormsView.EdgeThickness;

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (DeviceInfo.Version.Major < 11)
                {
                    On<iOS>().SetUseSafeArea(true);
                }
                else
                {
                    _CropImageFormsView.SafeAreaPadding = On<iOS>().SafeAreaInsets();
                    if (_CropImageFormsView.SafeAreaPadding.IsEmpty)
                    {
                        _CropImageFormsView.SafeAreaPadding = new Thickness(0, 30, 0, 0);
                    }
                }
            }

            //Color buttonColor = Color.Transparent;
            //_CropImageFormsView.MenuButton.BackgroundColor = buttonColor;
            //_CropImageFormsView.RotateLeftButton.BackgroundColor = buttonColor;
            //_CropImageFormsView.RotateRightButton.BackgroundColor = buttonColor;
            //_CropImageFormsView.CloseButton.BackgroundColor = buttonColor;
            //_CropImageFormsView.SelectButton.BackgroundColor = buttonColor;

            this.Content= _MainLayout;

#if __ANDROID__
            
            //(this.Handler.PlatformView as AndroidX.Core.View).SystemGestureExclusionRects()
#endif

            _Record.LoadPreferencies();

            if (InitMetaImage != null) OpenImage("", InitMetaImage);
#if DEBUG2
            
            if (b_DebugFirstStart)// && Device.RuntimePlatform == Device.iOS)
            {
                b_DebugFirstStart = false;
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(1000);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Scan_ClickedAsync();
                    });
                });
            }
#endif
        }
              
        void _CropImageFormsView_OnMenu(object sender, EventArgs e)
        {
            var menu = CreateMenuItems();
            var page = new Popup.MainMenuDialogPage(menu, _ToolBarLayout.Height);
            page.ShowDialog();
        }

        protected override bool OnBackButtonPressed()
        {
            if (_CropImageFormsView.Active)
            {
                _CropImageFormsView.Active = false;
                return true;
            }
            return false;
        }

        void StatusLog(string s)
        {
            Console.WriteLine(s);
        }

        List<MyMenuItem> CreateMenuItems()
        {
            List<MyMenuItem> result = new List<MyMenuItem>();

            result.Add(new MyMenuItem("Scan", Scan_ClickedAsync));
            result.Add(new MyMenuItem("Scan settigns", ScanSettings_ClickedAsync));
            result.Add(new MyMenuItem("Open", OpenImage_ClickedAsync));

            bool isImagePressent = _Record.IsSourceImagePrssent;

            if (isImagePressent)
            {
                result.Add(new MyMenuItem("Manual Crop", ManualCropButton_Clicked));
                result.Add(new MyMenuItem("Color Profile", CropButton_Clicked));
                result.Add(new MyMenuItem("Save to file",()=> SaveButton_Clicked(true)));
            }

            result.Add(new MyMenuItem("About", ()=>(new Popup.AboutDialogPage()).ShowDialog()));

            return result;
        }

        

        bool _bSkipActveChanged = false;
        private void _CropImageFormsView_ActveChanged(object sender, EventArgs e)
        {
            //Device.InvokeOnMainThreadAsync(() => _TopLayout.IsVisible = _CropImageFormsView.Active);
            if (!_CropImageFormsView.Active)
            {
                if (!_bSkipActveChanged)
                {
                    DoCropImage(false);
                }
            }
        }

        void ScanSettings_ClickedAsync()
        {
            (new Popup.CameraSettingsDialogPage(_ToolBarLayout.Height)).ShowDialog();
        }

        void Scan_ClickedAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var p = new CameraMainPage() { OldMetaImage = _Record.SourceImage };
                p.SafeAreaPadding = _CropImageFormsView.SafeAreaPadding;
                Microsoft.Maui.Controls.Application.Current.MainPage = p;
            }
            );
        }


        #region OpenImage
        async void OpenImage_ClickedAsync()
        {
            try
            {
                BeforeProcess();
                var options = PickOptions.Images;
                //options.PickerTitle = "Please select a picture file";
#if __IOS__
            
                iOSFileDialog();
#else

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    OpenImage(result.FullPath, null);
                }
                else
                {
                    UpdateView(false, true, false);
                }
#endif
            }
            catch (Exception ex)
            {
                MyTools.MessageBox("Error!", ex.ToString());
            }
        }

#if __IOS__
        void iOSFileDialog()
        {
            //https://forums.xamarin.com/discussion/104295/how-to-open-files-app-ios-11-programmatically
            var docPicker = new UIKit.UIDocumentPickerViewController(new string[] { MobileCoreServices.UTType.Data, MobileCoreServices.UTType.Image }, UIKit.UIDocumentPickerMode.Import);
            
            docPicker.DidPickDocumentAtUrls += (object sender, UIKit.UIDocumentPickedAtUrlsEventArgs e) =>
            {
                string filename =  e.Urls[0].Path;
                
                if (filename != null)
                {
                    OpenImage(filename, null);
                }
                else
                {
                    UpdateView(false, true, false);
                }
            };

            UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(docPicker, true, null);
        }
#endif
        void OpenDemo()
        {
            OpenImage("/storage/emulated/0/sdcard/Download/demo2l.jpg", null);
        }

        public void OpenImage(string uri, MetaImage source)
        {
            StatusLog("Opening image. Please wait...");

            DateTime start = DateTime.UtcNow;

            bool old = _Record.IsSourceImagePrssent;
            BeforeProcess();

#if DEBUG
            Console.WriteLine("open uri " + uri.ToString());
#endif

            Task.Run(() =>
            {

                _Record.OpenSourceImage(uri, source);


                if (!string.IsNullOrEmpty(_Record.LastStateText))
                {

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MyTools.MessageBox(_Record.LastStateText);
                    });
                }

                if (!_Record.DetectedDocumentCorners.IsInited)
                {
                    DoCropImage(true);
                }
                else
                {
                    UpdateView(old != _Record.IsSourceImagePrssent, true, false);
                }

                StatusLog("Opened " + ((int)(DateTime.UtcNow - start).TotalMilliseconds).ToString() + "ms");
            });
        }

#endregion OpenImage

        #region Crop

        void CropButton_Clicked()
        {
            Task.Run(() =>
            {
            MainThread.BeginInvokeOnMainThread( () =>
            {
                (new Popup.ProfileDialogPage(this, _ToolBarLayout.Height)).ShowDialog();
            });
            });
        }

        void ManualCropButton_Clicked()
        {
            DoCropImage(true);
        }

        public void DoCropImage(bool source)
        {
            BeforeProcess();
            StatusLog("Processing...");
            Task.Run(() =>
            {
                //if (source)
                //{
                //    // Special case: reset to source
                //    _Record.OnShowSource();
                //}
                //else
                {
                    _Record.OnCropImage(!source);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateView(false, true, source);
                });

            });
        }

        #endregion


        #region Save image
        void SaveButton_Clicked(bool needShare)
        {
            var page = new Popup.SaveDialogPage(this,_ToolBarLayout.Height);
            page.ShowDialog();
        }

        internal async void DoSaveImage(SaveImageTask.Params prm, bool needShare)
        {
            //for local folder
            if (true)//await CheckPermission())
            {
                _ = Task.Run(() =>
                {

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusLog("Saving image. Please wait...");
                        BeforeProcess();
                    });

                    _Record.WriterType = prm.writerType;
                    _Record.PaperFormat = prm.paperFormat;
                    _Record.MultiPages = prm.multiPages;
                    _Record.CustomPageWidth = prm.CustomPageWidth;
                    _Record.CustomPageHeight = prm.CustomPageHeight;

                    string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

#if DEBUG2
                    needShare = false;


                    if (!needShare)
                    {
                        path = "/sdcard/Download/";

                        //var p = DependencyService.Get<Utils.IGetPicturesFolder>();
                        //if (p != null)
                        //{
                        //    path = p.GetPicturesFolder();
                        //}
                    }
#endif

                    prm.filePath = System.IO.Path.Combine(path, string.Format("Pixelnetica-SdkDemo-{0:X08}.jpg", DateTime.UtcNow.Ticks));
                    //*/
                    //string fileName = System.IO.Path.Combine(path, string.Format("temp.jpg", DateTime.UtcNow.Ticks));
                    //if (System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);

                    _Record.OnSaveImage(prm, needShare);
                    UpdateView(false, false, _CropImageFormsView.Active);
                });
            }
            else
            {
                MyTools.MessageBox("Error!", "No write access!");
            }
        }

        async Task<bool> CheckPermission()
        {
            PermissionStatus status = PermissionStatus.Unknown;
            try
            {
                status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                }
                return status == PermissionStatus.Granted;

            }
            catch (Exception ex)
            {
                //Something went wrong
                MyTools.MessageBox("Error!", ex.ToString());
            }

            return await Task.FromResult(status==PermissionStatus.Granted);
        }

        #endregion Save image

        #region Update picture and state

        DateTime _StartUtcTine = DateTime.UtcNow;

        void UpdateView(bool needUpdateMenu, bool needUpdateMetaImage, bool editMode)
        {
            DateTime updateTime = DateTime.UtcNow;

                       
            //bool editMode = _Record.ImageMode == MainRecord.ImageState.Source;
            if (needUpdateMetaImage)
            {
                _CropImageFormsView.UiSetMetaImage(_Record.DisplayBitmap, !editMode, _Record.DocumentCorners, _Record.DetectedDocumentCorners);
            }

            _bSkipActveChanged = true;
            //_CropImageFormsView.ActveChanged -= _CropImageFormsView_ActveChanged; //not update second time
            _CropImageFormsView.Active = editMode;
            _bSkipActveChanged = false;
            //_CropImageFormsView.ActveChanged += _CropImageFormsView_ActveChanged;

            var now = DateTime.UtcNow;
            StatusLog(string.Format("[{0}+{1}ms] {2}", (int)(now - _StartUtcTine).TotalMilliseconds,
                (int)(now - updateTime).TotalMilliseconds, _Record.LastStateText));
            //});
        }


        void BeforeProcess()
        {
            _StartUtcTine = DateTime.UtcNow;
        }
        #endregion Update picture and state

        void LicenseInfo_Clicked()
        {
            var info = Main.GetLicenseInfo();
            if (info == null) MyTools.MessageBox("None");
            else
            {
                string msg = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5:X}", info.Status, info.ClientName, info.ClientExtraInfo, info.UtcValidSubscriptionTs, info.UtcValidTs, info.Features);
                MyTools.MessageBox(msg);
            }
        }

        void Test_Clicked()
        {
            var info = Main.GetLicenseInfo();
            if (info == null) MyTools.MessageBox("None");
            else
            {
                string msg = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5:X}", info.Status, info.ClientName, info.ClientExtraInfo, info.UtcValidSubscriptionTs, info.UtcValidTs, info.Features);
                MyTools.MessageBox(msg);
            }
        }
    }
}
