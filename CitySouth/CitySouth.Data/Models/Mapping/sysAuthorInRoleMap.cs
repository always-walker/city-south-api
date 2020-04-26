using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysAuthorInRoleMap : EntityTypeConfiguration<sysAuthorInRole>
    {
        public sysAuthorInRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.AuthorInRoleId);

            // Properties
            // Table & Column Mappings
            this.ToTable("sysAuthorInRole");
            this.Property(t => t.AuthorInRoleId).HasColumnName("AuthorInRoleId");
            this.Property(t => t.AuthorId).HasColumnName("AuthorId");
            this.Property(t => t.RoleId).HasColumnName("RoleId");
        }
    }
}
