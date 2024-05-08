using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsDemoApplication.Popup
{
    internal class ProfileDialogPage : MyBaseDialogPage
    {
        MainPage _Owner;
        EProcessing _CurrentProfile;
        MyCheckBox _StrongShadows;

        public ProfileDialogPage(MainPage a,double top):base(top)
        {
            _Owner = a;

            MainLayout.Spacing = MenuDivSize;

            var profilesLayout = new StackLayout();
            var label = new MyLabel(true, "Color Profile");
            profilesLayout.Children.Add(label);
            MainLayout.Children.Add(profilesLayout);

            _CurrentProfile = _Owner._Record.LastProcessingMode;
            foreach (var it in Processing.Instance.Profiles)
            {
                var p = new MyRadioButton(it.Value, _CurrentProfile == it.Key);
                p.CheckedChanged += R_CheckedChanged;
                MainLayout.Children.Add(p);
            }

            MainLayout.Children.Add(_StrongShadows = new MyCheckBox("Strong shadows", a._Record.StrongShadow));
                        
            _StrongShadows.CheckedChanged += _StrongShadows_CheckedChanged;
        }

        private void _StrongShadows_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            _Owner._Record.StrongShadow = e.Value;
            DoCropImage();
        }

        private void R_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return ;

            string value = ((MyRadioButton)sender).TextValue;
            foreach(var p in Processing.Instance.Profiles)
            {
                if(p.Value==value)
                {
                    _CurrentProfile=p.Key;
                    _Owner._Record.LastProcessingMode = p.Key;
                    DoCropImage();
                    break;
                }
            }
        }

        void DoCropImage()
        {
            try
            {
                this.CloseDialog();
            }
            catch { };

            _Owner.DoCropImage(false);
        }
    }
}
