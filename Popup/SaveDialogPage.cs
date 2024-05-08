using ImageSdkWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsDemoApplication.Popup
{
    internal class SaveDialogPage : MyBaseDialogPage
    {
        MainPage _Owner;

        MyCheckBox _MultiplePagesCB;
        StackLayout _PaperFormatLayout=new StackLayout();
        ImageWriter.EImageFileType _CurrentImageFileType;
        ImageWriter.EPaperFormatConfigValues _CurrentPaperFormat;

        Label _WidthLabel;
        Label _HeightLabel;
        Entry _WidthEditor;
        Entry _HeightEditor;

        Dictionary<ImageWriter.EImageFileType, string> _Types = new Dictionary<ImageWriter.EImageFileType, string>();
        Dictionary<ImageWriter.EPaperFormatConfigValues, string> _PaperFormats = new Dictionary<ImageWriter.EPaperFormatConfigValues, string>();

        public SaveDialogPage(MainPage a,double top):base(top)
        {
            _Owner = a;
            MainLayout.Spacing = MenuDivSize;

            _Types.Add(ImageWriter.EImageFileType.Jpeg, "JPEG");
            _Types.Add(ImageWriter.EImageFileType.Tiff, "TIFF G4");
            _Types.Add(ImageWriter.EImageFileType.Png, "PNG");
            //_Types.Add(ImageWriter.EImageFileType.PngExt, "PNG (SDK)");
            _Types.Add(ImageWriter.EImageFileType.Pdf, "PDF");
            _Types.Add(ImageWriter.EImageFileType.PdfPng, "PDF/PNG");

            _PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.A4, "A4");
            _PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.A5, "A5");
            _PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.A6, "A6");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.HalfLetter, "Predefined (half letter)");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.Legal, "Legal)");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.JuniorLegal, "Custom (eq. A5)");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.Leger, "Leger");
            _PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.BusinessCard, "Business Card");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.BusinessCard2, "BusinessCard2");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.ReceiptMobile, "ReceiptMobile");
            //_PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.ReceiptStation, "ReceiptStation");
            _PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.ReceiptKitchen, "Receipt");
            _PaperFormats.Add(ImageWriter.EPaperFormatConfigValues.Terminator, "Custom in mm");


                        
            var fileFormats = new StackLayout();
            var label = new MyLabel(true,"Save format") { Margin = new Thickness(0, 0, 0, 10) };

            fileFormats.Children.Add(label);            

            _CurrentImageFileType = _Owner._Record.WriterType;
            if (_CurrentImageFileType == ImageWriter.EImageFileType.PngExt) _CurrentImageFileType = ImageWriter.EImageFileType.Png;
            fileFormats.Spacing = 0;
            foreach (var it in _Types.Keys)
            {
                var r = new MyRadioButton(_Types[it], _CurrentImageFileType == it);
                r.CheckedChanged += ImageType_CheckedChanged;
                fileFormats.Children.Add(r);
            }

            MainLayout.Children.Add(fileFormats);

            _PaperFormatLayout.Children.Add(new MyLabel(true, "Some PDF page configurations") { Margin = new Thickness(0, 0, 0, 5) });

            _PaperFormatLayout.Spacing = 0;
            _CurrentPaperFormat = _Owner._Record.PaperFormat;
            foreach (var it in _PaperFormats)
            {
                var r = new MyRadioButton(it.Value, _CurrentPaperFormat == it.Key);
                r.CheckedChanged += PaperFornat_CheckedChanged;
                _PaperFormatLayout.Children.Add(r);
            }
            

            var g = new Grid();
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            _WidthLabel = new MyLabel(true, "Width");
            _HeightLabel = new MyLabel(true, "Height");
            _WidthEditor = new Entry() { FontSize = MenuDescriptionFontSize };
            _HeightEditor = new Entry() {FontSize = MenuDescriptionFontSize };

            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = 100 });
            g.Add(_WidthLabel, 0, 0);
            g.Add(_HeightLabel, 0, 1);

            _WidthEditor.Text = _Owner._Record.CustomPageWidth.ToString();
            _HeightEditor.Text = _Owner._Record.CustomPageHeight.ToString();
            g.Add(_WidthEditor, 1, 0);
            g.Add(_HeightEditor, 1, 1);
            UpdateEditorsState();

            _PaperFormatLayout.Children.Add(g);
            _PaperFormatLayout.Margin = new Thickness(0, 10, 0, 0);
            MainLayout.Children.Add(_PaperFormatLayout);

            MainLayout.Children.Add(_MultiplePagesCB=new MyCheckBox("Simulate multiple pages",a._Record.MultiPages));

            var ok = new Button() { Text = "OK", HorizontalOptions = LayoutOptions.End, FontSize = MenuFontSize, Margin = new Thickness(MenuPadding) };
            ok.Clicked += OnOK;
            MainLayout.Children.Add(ok);

            UpdateVisible();
        }

        async private void OnOK(object sender, EventArgs e)
        {
            SaveImageTask.Params p = new SaveImageTask.Params();
            p.writerType = _CurrentImageFileType;
            p.multiPages = _MultiplePagesCB.IsChecked;
            p.paperFormat = _CurrentPaperFormat;
            int.TryParse(_WidthEditor.Text,out p.CustomPageWidth);
            int.TryParse(_HeightEditor.Text, out p.CustomPageHeight);

            if (p.writerType == ImageWriter.EImageFileType.Png)
            {
                if (_Owner._Record.LastProcessingMode == EProcessing.BW)
                {
                    p.writerType = ImageWriter.EImageFileType.PngExt;
                }
            }
            
            try
            {
                this.CloseDialog();
            }
            catch { };
                        

            _Owner.DoSaveImage(p, true);
        }


        private void ImageType_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                string name = ((string)((RadioButton)sender).Content).Trim();
                foreach (var p in _Types)
                {
                    if (p.Value == name)
                    {
                        _CurrentImageFileType = p.Key;
                        break;
                    }
                }

                UpdateVisible();
            }
        }

        void UpdateVisible()
        {
            bool multi= (_CurrentImageFileType == ImageWriter.EImageFileType.Pdf) || (_CurrentImageFileType == ImageWriter.EImageFileType.PdfPng)
                || (_CurrentImageFileType == ImageWriter.EImageFileType.Tiff);
            bool pf = (_CurrentImageFileType == ImageWriter.EImageFileType.Pdf) || (_CurrentImageFileType == ImageWriter.EImageFileType.PdfPng);
#if _FORMS_
            _MultiplePagesCB.IsVisible = multi;
            _PaperFormatLayout.IsVisible = pf;
#else

            _MultiplePagesCB.IsVisible = multi;
            _PaperFormatLayout.IsVisible = pf;

            _MultiplePagesCB.Label.IsEnabled = multi;

            base.VerticalOptions = pf ? Microsoft.Maui.Primitives.LayoutAlignment.Center: Microsoft.Maui.Primitives.LayoutAlignment.Fill;
            

            //_PaperFormatLayout.IsEnabled = pf;
            //foreach (var p in _PaperFormatLayout.Children) 
            //{
            //    if (p is MyRadioButton) ((MyRadioButton)p).IsEnabled = pf;
            //    //p.IsEnabled = pf;
            //}
            //_WidthLabel.IsEnabled = pf;
            //_HeightLabel.IsEnabled = pf;
            //_WidthEditor.IsEnabled = pf;
            //_HeightEditor.IsEnabled = pf;
#endif

        }

        private void PaperFornat_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                string name = ((string)((RadioButton)sender).Content).Trim();
                foreach (var p in _PaperFormats)
                {
                    if (p.Value == name)
                    {
                        _CurrentPaperFormat = p.Key;
                        UpdateEditorsState();
                        break;
                    }
                }
            }
        }

        void UpdateEditorsState()
        {
            bool custom = _CurrentPaperFormat == ImageWriter.EPaperFormatConfigValues.Terminator;
            _HeightEditor.IsEnabled = custom;
            _WidthEditor.IsEnabled = custom;
            _WidthLabel.IsEnabled = custom;
            _HeightLabel.IsEnabled = custom;
        }

        
    }
}
