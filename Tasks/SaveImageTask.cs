using System;
using System.IO;
using System.Threading.Tasks;
using ImageSdkWrapper;

namespace XamarinFormsDemoApplication
{
    class SaveImageTask 
    {
        // Input
        public class Params
        {
            public MetaImage image;
            public string filePath;
            public ImageWriter.EImageFileType writerType;
            public ImageWriter.EPaperFormatConfigValues paperFormat;
            public bool multiPages;
            public int CustomPageWidth, CustomPageHeight;

            public Params() { }
        }

        // Output
        public class Result
        {
            public  string outputFilePath;
            public  long outputFileSize;
            public  string errorMessage;
                                    
            public bool HasError { get => !string.IsNullOrEmpty(errorMessage); }
        }
                
        static string GetExt(ImageWriter.EImageFileType type,string defFilePath)
        {            
            switch (type)
            {
                case ImageWriter.EImageFileType.Jpeg: return ".jpg";                    
                case ImageWriter.EImageFileType.Png:
                case ImageWriter.EImageFileType.PngExt:return ".png";
                case ImageWriter.EImageFileType.WebM: return".webm";
                case ImageWriter.EImageFileType.PdfPng:
                case ImageWriter.EImageFileType.Pdf:return  ".pdf";
                case ImageWriter.EImageFileType.Tiff:return ".tif";
                default:
                    return Path.GetExtension(defFilePath);
            }
        }

        public static Task<Result> Do(Params input)
        {
            Result result = new Result();
            
            try
            {
                // Change input file extensions and setup simple config params
                string extensions=GetExt(input.writerType, Path.GetExtension(input.filePath));

                int maxPages = 1;
                var filePath = Path.ChangeExtension(input.filePath, extensions);
                using (ImageWriter writer = new ImageWriter(input.writerType))
                {
                    writer.Open(filePath);

                    switch (input.writerType)
                    {
                        case ImageWriter.EImageFileType.Jpeg:
                            writer.Configure(ImageWriter.EConfigParam.CompressionQuality, 80);
                            //bundle.PutInt(ImageWriter.ConfigCompression, 80);
                            break;

                        case ImageWriter.EImageFileType.Png:
                            break;
                        case ImageWriter.EImageFileType.PngExt:
                            break;

                        case ImageWriter.EImageFileType.WebM:
                            break;

                        case ImageWriter.EImageFileType.PdfPng:
                        case ImageWriter.EImageFileType.Pdf:
                            maxPages = 3;

                            if (input.paperFormat == ImageWriter.EPaperFormatConfigValues.Terminator)
                            {
                                //units before size
                                writer.Configure(ImageWriter.EConfigParam.Units, ImageWriter.EUnitsConfigValues.Millimeters);
                                writer.Configure(ImageWriter.EConfigParam.PageWidth, input.CustomPageWidth);
                                writer.Configure(ImageWriter.EConfigParam.PageHeight, input.CustomPageHeight);                                
                            }
                            else
                            {
                                writer.Configure(ImageWriter.EConfigParam.Paper, input.paperFormat);
                                
                            }
                            
                            writer.Configure(ImageWriter.EConfigParam.FooterHeight, 10);
                            //writer.Configure(ImageWriter.EConfigParam.FooterText, "Test");
                            break;

                        case ImageWriter.EImageFileType.Tiff:
                            maxPages = 3;
                            break;

                        default:
                            break;
                    }

                    //writer.Configure().Configure(bundle);
                    // Simulate multipages
                    int pageCount = input.multiPages ? maxPages : 1;
                    Console.WriteLine("Before write -1 "+pageCount.ToString()+" - "+input.writerType.ToString());
                    string pngFileName = System.IO.Path.ChangeExtension(filePath, ".png");
                    for (int i = 0; i < pageCount; ++i)
                    {
                        if (input.writerType == ImageWriter.EImageFileType.PdfPng)
                        {
                            if (i == 0)
                            {
                                using (ImageWriter subWriter = new ImageWriter(ImageWriter.EImageFileType.PngExt))
                                {
                                    subWriter.Open(pngFileName);
                                    pngFileName = subWriter.Write(input.image);
                                }
                            }
                                                        
                            var filePath2=writer.WriteFile(pngFileName, ImageWriter.EPngPdfImageFileType.Png, input.image.ExifOrientation);
                        }
                        else
                        {
                            writer.Write(input.image);
                        }
                    }
                    Thread.Sleep(500);
                }

                long outputFileSize = new FileInfo(filePath).Length;

                // Build result
                result.outputFilePath = filePath;
                result.outputFileSize = outputFileSize;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                result.errorMessage = e.Message;
            }

            return Task.FromResult(result);
        }

        
    }
}