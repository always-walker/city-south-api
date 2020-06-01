using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class OwnerPropertyExpireLogMap : EntityTypeConfiguration<OwnerPropertyExpireLog>
    {
        public OwnerPropertyExpireLogMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);

            // Properties
            this.Property(t => t.UserName)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("OwnerPropertyExpireLog");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.ExpireDate).HasColumnName("ExpireDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
        }
    }
}
