using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clone_app_Zing_Mp3
{
    public class Product
    {
        public string NameMusic { get; set; }
        public string Image { get; set; }
        public Product(string name,string image)
        {
            NameMusic = name;
            Image = image;
        }

    }
}
