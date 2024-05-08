using ImageSdkWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsDemoApplication
{
    class CropImageTask 
    {
        public class Params
        {
            public readonly MetaImage Image;
            public readonly bool StrongShadows;
            public readonly Corners Corners;
            public readonly EProcessing Processing;
            public readonly bool bNeedCrop;

            public Params(MetaImage image, bool strongShadows, Corners corners, EProcessing processing, bool bNeedCrop)
            {
                this.Image = image;
                this.StrongShadows = strongShadows;
                this.Corners = corners;
                this.Processing = processing;
                this.bNeedCrop = bNeedCrop;
            }
        }

        internal class Result
        {
            public MetaImage Image;
            public bool StrongShadows;
            public Corners Corners;
            public string Error;
            public EProcessing Processing;

            public string errorMessage=null;
            public bool HasError { get => !string.IsNullOrEmpty(errorMessage); }
        }
        public static Task<Result> Do(Params inputParams)
        {
            Result result = new Result();

            try
            {                
                // Working with ImageSDK
                using (ImageProcessing sdk = new ImageProcessing())
                {
                    // Crop image
                    
                    MetaImage croppedImage;
                    if ((inputParams.Corners != null)&&(inputParams.bNeedCrop))// Processing!=EProcessing.Source))
                    {
                        croppedImage = sdk.CorrectDocument(inputParams.Image, inputParams.Corners);
                    }
                    else
                    {
                        // Corners wasn't defined
                        croppedImage = inputParams.Image;
                    }

                    if (croppedImage == null)
                    {
                        // Something wrong
                        result.Error = "Cannot crop input image";
                    }
                    else
                    {
                        croppedImage.StrongShadows = inputParams.StrongShadows;

                        // Process
                        MetaImage targetImage = null;
                        switch (inputParams.Processing)
                        {
                            case EProcessing.Original:
                                targetImage = sdk.ImageOriginal(croppedImage);
                                break;

                            case EProcessing.BW:
                                targetImage = sdk.ImageBWBinarization(croppedImage);
                                break;

                            case EProcessing.Gray:
                                targetImage = sdk.ImageGrayBinarization(croppedImage);
                                break;

                            case EProcessing.Color:
                                targetImage = sdk.ImageColorBinarization(croppedImage);
                                break;                            
                        }
                        
                        // Check processing error
                        if (targetImage == null)
                        {
                            result.Error = string.Format("Failed to perform processing {0}", inputParams.Processing);
                        }
                        else
                        {
                            // Cleanup
                            if (!croppedImage.Equals(inputParams.Image))
                            {
                                MetaImage.SafeRecycleBitmap(croppedImage, targetImage);
                            }
                            
                            //if(result.Image!=null)
                            //{
                            //    MetaImage.SafeRecycleBitmap(result.Image, targetImage);
                            //}

                            result.Image = targetImage;

                            GC.Collect();

                            result.StrongShadows = inputParams.StrongShadows;
                            result.Corners = null;
                            result.Processing = inputParams.Processing;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MyTools.MessageBox("Error!", e.Message);                
            }
            return Task.FromResult(result);
        }
    }
}