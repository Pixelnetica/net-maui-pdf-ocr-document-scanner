using ImageSdkWrapper.Forms.Camera;
using ImageSdkWrapper.Maui.Camera;

using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsDemoApplication.Popup
{
    internal class CameraSettingsDialogPage :  MyBaseDialogPage
    {
        MyCheckBox ShakeDetection = new MyCheckBox("Shake detection",Settings.ShakeDetection);
        MyCheckBox DocumentArea = new MyCheckBox("Document area", Settings.DocumentArea);
        MyCheckBox TrapezoidDistortion = new MyCheckBox("Trapezoid distortion", Settings.TrapezoidDistortion);
        public CameraSettingsDialogPage(MainPage p, double top):base(p,top)
        {
            MainLayout.Children.Add(ShakeDetection);
            MainLayout.Children.Add(DocumentArea);
            MainLayout.Children.Add(TrapezoidDistortion);

            ShakeDetection.CheckedChanged += ShakeDetection_CheckedChanged;
            DocumentArea.CheckedChanged += DocumentArea_CheckedChanged;
            TrapezoidDistortion.CheckedChanged += TrapezoidDistortion_CheckedChanged;
        }

        private void TrapezoidDistortion_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Settings.TrapezoidDistortion = TrapezoidDistortion.IsChecked;
        }

        private void DocumentArea_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Settings.DocumentArea = DocumentArea.IsChecked;
        }

        private void ShakeDetection_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Settings.ShakeDetection = ShakeDetection.IsChecked;
        }
    }
}
