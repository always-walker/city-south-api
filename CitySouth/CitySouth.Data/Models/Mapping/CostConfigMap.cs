using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class CostConfigMap : EntityTypeConfiguration<CostConfig>
    {
        public CostConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.ConfigId);

            // Properties
            this.Property(t => t.ConfigType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ConfigVersion)
                .HasMaxLength(50);

            this.Property(t => t.Category)
                .HasMaxLength(50);

            this.Property(t => t.UnitName)
                .HasMaxLength(50);

            this.Property(t => t.DiscountMode)
                .HasMaxLength(20);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("CostConfig");
            this.Property(t => t.ConfigId).HasColumnName("ConfigId");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.ConfigType).HasColumnName("ConfigType");
            this.Property(t => t.ConfigVersion).HasColumnName("ConfigVersion");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.UnitName).HasColumnName("UnitName");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.IsDefault).HasColumnName("IsDefault");
            this.Property(t => t.IsAble).HasColumnName("IsAble");
            this.Property(t => t.DiscountMode).HasColumnName("DiscountMode");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
