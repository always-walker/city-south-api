using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysUserInRoleMap : EntityTypeConfiguration<sysUserInRole>
    {
        public sysUserInRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.UserInRoleId);

            // Properties
            // Table & Column Mappings
            this.ToTable("sysUserInRole");
            this.Property(t => t.UserInRoleId).HasColumnName("UserInRoleId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.RoleId).HasColumnName("RoleId");
        }
    }
}
