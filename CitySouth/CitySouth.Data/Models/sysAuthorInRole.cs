using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class sysAuthorInRole
    {
        public int AuthorInRoleId { get; set; }
        public int AuthorId { get; set; }
        public int RoleId { get; set; }
    }
}
