using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysConfigMap : EntityTypeConfiguration<sysConfig>
    {
        public sysConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.ConfigId);

            // Properties
            this.Property(t => t.ConfigKey)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ConfigKeyKeyName)
                .HasMaxLength(50);

            this.Property(t => t.ConfigValue)
                .HasMaxLength(250);

            this.Property(t => t.DataType)
                .HasMaxLength(50);

            this.Property(t => t.CheckType)
                .HasMaxLength(50);

            this.Property(t => t.GroupName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            this.Property(t => t.ErrorMsg)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("sysConfig");
            this.Property(t => t.ConfigId).HasColumnName("ConfigId");
            this.Property(t => t.ParentConfigId).HasColumnName("ParentConfigId");
            this.Property(t => t.ConfigKey).HasColumnName("ConfigKey");
            this.Property(t => t.ConfigKeyKeyName).HasColumnName("ConfigKeyKeyName");
            this.Property(t => t.ConfigValue).HasColumnName("ConfigValue");
            this.Property(t => t.DataType).HasColumnName("DataType");
            this.Property(t => t.CheckType).HasColumnName("CheckType");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.ErrorMsg).HasColumnName("ErrorMsg");
            this.Property(t => t.Ignore).HasColumnName("Ignore");
            this.Property(t => t.Sort).HasColumnName("Sort");
        }
    }
}
