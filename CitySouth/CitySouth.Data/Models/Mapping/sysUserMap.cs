using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysUserMap : EntityTypeConfiguration<sysUser>
    {
        public sysUserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.LoginName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UserName)
                .HasMaxLength(50);

            this.Property(t => t.Sex)
                .HasMaxLength(10);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.HeadUrl)
                .HasMaxLength(50);

            this.Property(t => t.LastLoginIp)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("sysUser");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.LoginName).HasColumnName("LoginName");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Sex).HasColumnName("Sex");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.HeadUrl).HasColumnName("HeadUrl");
            this.Property(t => t.IsSuper).HasColumnName("IsSuper");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.LastLoginTime).HasColumnName("LastLoginTime");
            this.Property(t => t.LastLoginIp).HasColumnName("LastLoginIp");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
