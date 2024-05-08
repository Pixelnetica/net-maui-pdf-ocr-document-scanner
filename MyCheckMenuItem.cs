using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsDemoApplication
{
    internal class MyCheckMenuItem: MenuItem
    {
        bool _checked = false;
        public bool IsChecked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value; UpdateCheckImage();
                    if (OnCheckedChange != null) OnCheckedChange(this, _checked);
                }
            }
        }

        public delegate void CheckedEventHandler(object sender, bool ischecked);
        public event CheckedEventHandler OnCheckedChange;

        public MyCheckMenuItem()
        {
            UpdateCheckImage();
            Clicked += MyCheckBoxButton_Clicked;
        }

        void UpdateCheckImage()
        {
            base.IconImageSource= ImageSource.FromFile(_checked ? "view_check_checked_enabled.png" : "view_check_unchecked_enabled.png");
        }

        private void MyCheckBoxButton_Clicked(object sender, EventArgs e)
        {
            IsChecked = !IsChecked;
        }
    }
}
