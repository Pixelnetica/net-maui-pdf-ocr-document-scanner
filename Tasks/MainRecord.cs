using System;
using System.Collections.Generic;
using ImageSdkWrapper;

namespace XamarinFormsDemoApplication
{
    class MainRecord 
    {        
        internal string LastStateText = "";

        public enum ImageState
        {
            InitNothing,    // No image to show
            Source,         // Display source image
            Target,         // Display processing result
        };


        ImageState imageMode = ImageState.InitNothing;
        
        string sourceImageUri;
        MetaImage sourceImage=null;  // Store source to 
        public MetaImage SourceImage { get => sourceImage; }
        public bool IsSourceImagePrssent { get { return sourceImage != null; } }

        Corners initCorners;    // Corners detected by SDK
        Corners userCorners;    // Corners modified by user
        internal PxlPoint userImageSize;

        MetaImage targetImage;
        bool inCropTask;

        // Data
        const string PREFS_STRONG_SHADOWS = "STRONG_SHADOWS";
        const string PREFS_WRITER_TYPE = "WRITER_TYPE";
        const string PREFS_MULTI_PAGES = "MULTI_PAGES";
        const string PREFS_PAPER_FORMAT = "PAPER_FORMAT";
        const string PREFS_LastProcessingMode = "LastProcessingMode";
        const string PREFS_CustomPageWidth = "PageWidth";
        const string PREFS_CustomPageHeight = "PageHeight";

        bool strongShadows;
        ImageWriter.EImageFileType writerType;
        ImageWriter.EPaperFormatConfigValues paperFormat;
        bool multiPages=false;
        EProcessing _LastProcessingMode = EProcessing.BW;

        public MainRecord()
        {            
        }

        public MetaImage DisplayBitmap
        {
            get
            {
                switch (imageMode)
                {
                    case ImageState.InitNothing:
                        return null;

                    case ImageState.Source:
                        return sourceImage;

                    case ImageState.Target:
                        return targetImage;

                    default:
                        throw new InvalidOperationException(string.Format("Illegal image mode {0}", imageMode));
                }
            }
        }

        public Corners DocumentCorners { get => userCorners; }
        public Corners DetectedDocumentCorners { get => initCorners; }

        public ImageState ImageMode { get => imageMode; }
        
        // Special case to prevent spinner blinking
        public ImageState DisplayImageMode { get => inCropTask ? ImageState.Target : imageMode; }

        public bool HasMessages { get => !string.IsNullOrEmpty(LastStateText); }

      
        public async void OpenSourceImage(string uri,MetaImage image)
        {
            // Reset Source
            imageMode = ImageState.InitNothing;
            sourceImageUri = null;
            if(sourceImage!=null) sourceImage.Dispose();
            sourceImage = null;
            if(targetImage!=null) targetImage.Dispose();
            targetImage = null;

            initCorners = userCorners = null;

            
            // Reset messages
            LastStateText = "";
                        
            GC.Collect();

            // Load in the thread
            LoadImageTask.Result result = await LoadImageTask.Do(uri.ToString(),image);
            
            // Store source
            sourceImageUri = uri;
            sourceImage = result.Image;

            // Store corners
            initCorners = result.Corners;
            if (initCorners == null) initCorners = new Corners();
            userCorners = new Corners();
            userCorners.SetCorners(initCorners);

            // Display source
            imageMode = ImageState.Source;

            // Build message
            if (result.HasError)
            {
                LastStateText = result.Error;
            }
            else
            {
                userImageSize = result.Image.BitmapBounds;
                DoCropImageTask(true);
            }

        }

        
        public void OnCropImage(bool needCropImage)
        {
            if (sourceImage == null)
            {
                MyTools.MessageBox("Error!","Empty image for crop!");
                return;
            }

            MetaImage.SafeRecycleBitmap(targetImage, sourceImage);
            targetImage = null;
            LastStateText = "";

            DoCropImageTask(needCropImage);
        }

        async void DoCropImageTask(bool needCrop)
        {
            inCropTask = true;

            var result = await CropImageTask.Do(new CropImageTask.Params(sourceImage, strongShadows, userCorners,LastProcessingMode, needCrop));

            inCropTask = false;

            if (result.HasError)
            {
                MyTools.MessageBox("Error!", result.errorMessage);
            }
            else
            {
#if DEBUG
                Console.WriteLine("CropImage OK " +result.Image.Handle.ToString());
#endif

                // Display target
                targetImage = result.Image;
                imageMode = !needCrop ? ImageState.Source: ImageState.Target;
            }
        }

        async public void OnSaveImage(SaveImageTask.Params prm, bool needShare)
        {
            // Define image
            switch (imageMode)
            {
                case ImageState.Source:
                    prm.image = sourceImage;
                    break;

                case ImageState.Target:
                    prm.image = targetImage;
                    break;

                default:
                    prm.image = null;
                    break;
            }
            if (prm.image == null)
            {
                LastStateText = "Error! no image";
            }
            else
            {
                LastStateText = "";
                
                SaveImageTask.Result result = await SaveImageTask.Do(prm);

                
                if (result.HasError)
                {
                    LastStateText = result.errorMessage;
                    //fileName = result.errorMessage;
                }
                else
                {
                    LastStateText = result.outputFilePath;
                    if (needShare)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Share.RequestAsync(new ShareFileRequest
                            {
                                Title = "Share image file",
                                File = new ShareFile(result.outputFilePath)
                            });
                        });
                    }

                }
            }
        }

        public void OnShowSource()
        {
            imageMode = ImageState.Source;

            MetaImage.SafeRecycleBitmap(targetImage, sourceImage);
            targetImage = null;
            LastStateText = "";

           // ExecuteOnVisible(callback);
        }

        public void LoadPreferencies()
        {
            strongShadows = Preferences.Get(PREFS_STRONG_SHADOWS, strongShadows);
            writerType = (ImageWriter.EImageFileType)Preferences.Get(PREFS_WRITER_TYPE, (int)writerType);
            multiPages = Preferences.Get(PREFS_MULTI_PAGES, multiPages);
            _LastProcessingMode = (EProcessing)Preferences.Get(PREFS_LastProcessingMode, (int)_LastProcessingMode);
            paperFormat = (ImageWriter.EPaperFormatConfigValues)Preferences.Get(PREFS_PAPER_FORMAT, (int)paperFormat);

            _CustomPageWidth = Preferences.Get(PREFS_CustomPageWidth, 200);
            _CustomPageHeight =Preferences.Get(PREFS_CustomPageHeight, 100);
            
        }


        public EProcessing LastProcessingMode
        {
            get => _LastProcessingMode; set {
                if (_LastProcessingMode != value)
                {
                    _LastProcessingMode = value;
                    Preferences.Set(PREFS_LastProcessingMode, (int)_LastProcessingMode);
                }
            }
        }

        public bool StrongShadow
        {
            get => strongShadows;
            set
            {
                if (value != strongShadows)
                {
                    strongShadows = value;
                    Preferences.Set(PREFS_STRONG_SHADOWS, strongShadows);
                }
            }
        }

        public ImageWriter.EImageFileType WriterType
        {
            get => writerType;
            set
            {
                if (value != writerType)
                {
                    writerType = value;
                    Preferences.Set(PREFS_WRITER_TYPE, (int)writerType);
                }
            }
        }

        public ImageWriter.EPaperFormatConfigValues PaperFormat
        {
            get => paperFormat;
            set
            {
                if (value != paperFormat)
                {
                    paperFormat = value;
                    Preferences.Set(PREFS_PAPER_FORMAT, (int)paperFormat);
                }
            }
        }

        public bool  MultiPages
        {
            get => multiPages;
            set
            {
                if (value != multiPages)
                {
                    multiPages = value;
                    Preferences.Set(PREFS_MULTI_PAGES, multiPages);
                }
            }
        }

        int _CustomPageWidth = 200;
        public int CustomPageWidth
        {
            get => _CustomPageWidth;
            set
            {
                if (value != _CustomPageWidth)
                { 
                    _CustomPageWidth= value;
                    Preferences.Set(PREFS_CustomPageWidth, value);
                }
            }
        }

        int _CustomPageHeight = 200;
        public int CustomPageHeight
        {
            get => _CustomPageHeight;
            set
            {
                if (value != _CustomPageHeight)
                {
                    _CustomPageHeight = value;
                    Preferences.Set(PREFS_CustomPageHeight, value);
                }
            }
        }

    }
}