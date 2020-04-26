using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysUserLogMap : EntityTypeConfiguration<sysUserLog>
    {
        public sysUserLogMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);

            // Properties
            this.Property(t => t.LoginName)
                .HasMaxLength(50);

            this.Property(t => t.LogType)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(1000);

            this.Property(t => t.Ip)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("sysUserLog");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.LoginName).HasColumnName("LoginName");
            this.Property(t => t.LogType).HasColumnName("LogType");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Ip).HasColumnName("Ip");
            this.Property(t => t.LogTime).HasColumnName("LogTime");
        }
    }
}
