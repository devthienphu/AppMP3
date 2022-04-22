using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clone_app_Zing_Mp3
{
    public class Menu
    {
        public string Title { get; set; }
        public PackIconKind Icon { get; set; }

        public Menu( string title,PackIconKind icon)
        {
            Title = title;
            Icon = icon;

        }
    }
}
