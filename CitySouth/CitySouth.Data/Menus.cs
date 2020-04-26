using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth.Data.Models
{
    public class Menus
    {
        public Menus(string _name, string _link, string _icon)
        {
            this.name = _name;
            this.link = _link;
            this.icon = _icon;
        }
        public string name { get; set; }
        public string link { get; set; }
        public string icon { get; set; }
        public List<Menus> children { get; set; }
    }
}
