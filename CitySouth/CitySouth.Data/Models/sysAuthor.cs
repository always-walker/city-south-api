using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class sysAuthor
    {
        public int AuthorId { get; set; }
        public int ParentAuthorId { get; set; }
        public string AuthorName { get; set; }
        public string MenuLink { get; set; }
        public string AuthorKey { get; set; }
        public string MenuIcon { get; set; }
        public int Sort { get; set; }
    }
}
