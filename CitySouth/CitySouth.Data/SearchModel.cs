using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth.Data
{
    public class SearchModel
    {
        public SearchModel() 
        {
            this.PageIndex = 1;
            this.PageSize = 10;
        }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int? FkId { get; set; }
        public int? Fk2Id { get; set; }
        public int? Fk3Id { get; set; }
        public string KeyWord { get; set; }
        public string type { get; set; }
        public DateTime? SearchDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCondition1 { get; set; }
        public bool? IsCondition2 { get; set; }
    }
}
