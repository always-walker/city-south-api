using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class WaterAndElectricityMap : EntityTypeConfiguration<WaterAndElectricity>
    {
        public WaterAndElectricityMap()
        {
            // Primary Key
            this.HasKey(t => t.FeeId);

            // Properties
            this.Property(t => t.PayerName)
                .HasMaxLength(50);

            this.Property(t => t.FeeType)
                .HasMaxLength(50);

            this.Property(t => t.FeeName)
                .HasMaxLength(50);

            this.Property(t => t.UnitName)
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
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WaterAndElectricity");
            this.Property(t => t.FeeId).HasColumnName("FeeId");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.PayerName).HasColumnName("PayerName");
            this.Property(t => t.ConfigId).HasColumnName("ConfigId");
            this.Property(t => t.FeeType).HasColumnName("FeeType");
            this.Property(t => t.FeeName).HasColumnName("FeeName");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.PayTime).HasColumnName("PayTime");
            this.Property(t => t.FeeDate).HasColumnName("FeeDate");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.UnitName).HasColumnName("UnitName");
            this.Property(t => t.LastQuantity).HasColumnName("LastQuantity");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.ShareQuantity).HasColumnName("ShareQuantity");
            this.Property(t => t.Amount).HasColumnName("Amount");
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
