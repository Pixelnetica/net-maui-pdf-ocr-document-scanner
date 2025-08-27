using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsDemoApplication.Popup
{
    internal class MyButton : Button
    {
        public MyButton()
        {
            TextColor = Colors.White;
            FontSize = MyBaseDialogPage.MenuFontSize;
            Margin = new Thickness(MyBaseDialogPage.MenuPadding);
            HorizontalOptions = LayoutOptions.End;
        }
    }
}
