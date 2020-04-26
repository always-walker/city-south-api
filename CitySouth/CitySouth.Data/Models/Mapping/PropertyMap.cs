using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class PropertyMap : EntityTypeConfiguration<Property>
    {
        public PropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.PropertyId);

            // Properties
            this.Property(t => t.PayerName)
                .HasMaxLength(50);

            this.Property(t => t.ReceiptNo)
                .HasMaxLength(50);

            this.Property(t => t.VoucherNo)
                .HasMaxLength(50);

            this.Property(t => t.OprationName)
                .HasMaxLength(50);

            this.Property(t => t.PayWay)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Property");
            this.Property(t => t.PropertyId).HasColumnName("PropertyId");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.PayerName).HasColumnName("PayerName");
            this.Property(t => t.ConfigId).HasColumnName("ConfigId");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.MonthCount).HasColumnName("MonthCount");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.PayTime).HasColumnName("PayTime");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.ReceiptNo).HasColumnName("ReceiptNo");
            this.Property(t => t.VoucherNo).HasColumnName("VoucherNo");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.OprationName).HasColumnName("OprationName");
            this.Property(t => t.PayWay).HasColumnName("PayWay");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
