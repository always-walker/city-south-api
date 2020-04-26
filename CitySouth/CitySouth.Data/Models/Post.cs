using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public int ParentPostId { get; set; }
        public string PostType { get; set; }
        public string PostName { get; set; }
    }
}
