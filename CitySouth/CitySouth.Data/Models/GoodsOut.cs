using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class GoodsOut
    {
        public int OutId { get; set; }
        public string OutNo { get; set; }
        public int EstateId { get; set; }
        public string Geter { get; set; }
        public System.DateTime GetDate { get; set; }
        public string Outer { get; set; }
        public string Passer { get; set; }
        public int PassStatus { get; set; }
        public Nullable<System.DateTime> PassDate { get; set; }
    }
}
