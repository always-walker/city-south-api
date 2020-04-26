using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysUserInEstateMap : EntityTypeConfiguration<sysUserInEstate>
    {
        public sysUserInEstateMap()
        {
            // Primary Key
            this.HasKey(t => t.UserInEstateId);

            // Properties
            // Table & Column Mappings
            this.ToTable("sysUserInEstate");
            this.Property(t => t.UserInEstateId).HasColumnName("UserInEstateId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
        }
    }
}
