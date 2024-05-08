using System.Collections.Generic;

namespace XamarinFormsDemoApplication
{
    public enum EProcessing
    {
        //Source,
        Original,
        BW,
        Gray,
        Color
    }
    public class Processing
    {
        public Dictionary<EProcessing, string> Profiles = new Dictionary<EProcessing, string>();

        public static Processing Instance = new Processing();
        Processing()
        {
            //Profiles.Add(EProcessing.Source, "Manual crop");
            Profiles.Add(EProcessing.Original, "Original");
            Profiles.Add(EProcessing.BW, "Black & White");
            Profiles.Add(EProcessing.Gray, "Gray");
            Profiles.Add(EProcessing.Color, "Color");
        }
    }
}