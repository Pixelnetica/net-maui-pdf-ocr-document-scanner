using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsDemoApplication.Popup
{
    internal class MyCheckBox : StackLayout
    {
        CheckBox _CB;
        Label _Label;
        public Label Label=>_Label;
        CheckBox CB=>_CB;

        public event EventHandler<CheckedChangedEventArgs> CheckedChanged;
        internal bool IsChecked { get => _CB.IsChecked; set => _CB.IsChecked = value; }
        public MyCheckBox(string text,bool value)
        {
            Orientation = StackOrientation.Horizontal;
            Children.Add(_CB = new CheckBox() { IsChecked = value, VerticalOptions = LayoutOptions.Center });
            _Label = new Label() { Text = text, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
            _Label.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { _CB.IsChecked = !_CB.IsChecked; }) });
            Children.Add(_Label);

            if (MyBaseDialogPage.MenuFontColor != null) _Label.TextColor= Popup.MyBaseDialogPage.MenuFontColor;
            _Label.FontSize = Popup.MyBaseDialogPage.MenuFontSize;
            _Label.HeightRequest= Popup.MyBaseDialogPage.MenuItemHeight;            
            _Label.VerticalTextAlignment = TextAlignment.Center;

            _CB.CheckedChanged += _CB_CheckedChanged;

            Padding = new Thickness(MyBaseDialogPage.MenuPadding, 0, MyBaseDialogPage.MenuPadding, 0);
        }

        private void _CB_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (CheckedChanged != null) CheckedChanged(this, e);
        }
    }

    internal class MyRadioButton : RadioButton
    {
        public string TextValue;
        public MyRadioButton(string text,bool value)
        {
            TextValue = text;
            Content = " "+text;
            IsChecked = value;
            base.BackgroundColor = MyBaseDialogPage.MenuBackColor;

            VerticalOptions = LayoutOptions.Center;
            FontSize = MyBaseDialogPage.MenuFontSize;
            if (MyBaseDialogPage.MenuFontColor != null) TextColor = MyBaseDialogPage.MenuFontColor;
            HeightRequest = MyBaseDialogPage.MenuItemHeight;
            Padding = new Thickness(MyBaseDialogPage.MenuPadding, 0, MyBaseDialogPage.MenuPadding, 0);
        }        
    }

    internal class MyLabel:Label
    {
        public MyLabel(bool isDescription,string text)
        {
            this.Text = text;
            VerticalOptions = LayoutOptions.Center;
            VerticalTextAlignment = TextAlignment.Center;
            FontSize = isDescription ? MyBaseDialogPage.MenuDescriptionFontSize:MyBaseDialogPage.MenuFontSize;
            
            if (MyBaseDialogPage.MenuFontColor!=null) TextColor = MyBaseDialogPage.MenuFontColor;
            Padding = new Thickness(MyBaseDialogPage.MenuPadding, 0, MyBaseDialogPage.MenuPadding, 0);
            if (!isDescription) HeightRequest = MyBaseDialogPage.MenuItemHeight;
        }
    }
}
