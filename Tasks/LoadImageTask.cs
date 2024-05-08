using ImageSdkWrapper;
using System;
using System.Threading.Tasks;

namespace XamarinFormsDemoApplication
{
    class LoadImageTask
    {
        internal class Result
        {
            public MetaImage Image;
            public Corners Corners;
            public string Error;

            public bool HasError { get => !string.IsNullOrEmpty(Error); }
            public bool HasCorners { get => this.Corners != null; }
        }
        
        public static System.Threading.Tasks.Task<Result> Do(string imageUri, MetaImage src)
        {
            Result result = new Result();
            try
            {
                // Open source image
                MetaImage sourceImage = null;

                if (src != null)
                {
                    sourceImage = src;
                }
                else
                {
                    using (System.IO.Stream stream = new System.IO.FileStream(imageUri, System.IO.FileMode.Open))//,System.IO.FileMode.Open,System.IO.FileAccess.Read,System.IO.FileShare.Read| System.IO.FileShare.Write))
                    {
                        sourceImage = MetaImage.FromStream(stream, imageUri);

                        if (sourceImage == null)
                        {
                            result.Error = string.Format("Cannot open image file {0}", imageUri);
                            return Task.FromResult(result);
                        }
                    }
                }
                // Scale image to supported size
                using (ImageProcessing sdk = new ImageProcessing())
                {
                    // Rotate to origin                                        
                    MetaImage originImage = sdk.ImageOriginal(sourceImage);

                    
                    //// Free source image
                    MetaImage.SafeRecycleBitmap(sourceImage, originImage);

                    Corners corners = sdk.DetectDocumentCorners(originImage);
                    
                    // Free memory
                    GC.Collect();

                    result.Image = originImage;
                    result.Corners = corners;
                }
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }
            return Task.FromResult(result);
        }
    }
}