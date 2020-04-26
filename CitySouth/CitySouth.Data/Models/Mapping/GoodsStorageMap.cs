using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodsStorageMap : EntityTypeConfiguration<GoodsStorage>
    {
        public GoodsStorageMap()
        {
            // Primary Key
            this.HasKey(t => t.StorageId);

            // Properties
            // Table & Column Mappings
            this.ToTable("GoodsStorage");
            this.Property(t => t.StorageId).HasColumnName("StorageId");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.GoodsId).HasColumnName("GoodsId");
            this.Property(t => t.Count).HasColumnName("Count");
            this.Property(t => t.Amount).HasColumnName("Amount");
        }
    }
}
