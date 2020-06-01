using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Article
    {
        public int ArticleId { get; set; }
        public string CategoryName { get; set; }
        public string EstateIds { get; set; }
        public string Title { get; set; }
        public string GoUrl { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Attachment { get; set; }
        public int Click { get; set; }
        public bool IsShow { get; set; }
        public System.DateTime AddTime { get; set; }
        public int Sort { get; set; }
    }
}
