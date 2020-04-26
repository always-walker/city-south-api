using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodsReceiptDetailMap : EntityTypeConfiguration<GoodsReceiptDetail>
    {
        public GoodsReceiptDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.DetailId);

            // Properties
            // Table & Column Mappings
            this.ToTable("GoodsReceiptDetail");
            this.Property(t => t.DetailId).HasColumnName("DetailId");
            this.Property(t => t.ReceiptId).HasColumnName("ReceiptId");
            this.Property(t => t.GoodsId).HasColumnName("GoodsId");
            this.Property(t => t.Count).HasColumnName("Count");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
        }
    }
}
