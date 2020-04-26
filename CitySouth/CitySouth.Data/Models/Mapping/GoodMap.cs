using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodMap : EntityTypeConfiguration<Good>
    {
        public GoodMap()
        {
            // Primary Key
            this.HasKey(t => t.GoodsId);

            // Properties
            this.Property(t => t.GoodsId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GoodsNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GoodsImage)
                .HasMaxLength(250);

            this.Property(t => t.GoodsName)
                .HasMaxLength(250);

            this.Property(t => t.Model)
                .HasMaxLength(50);

            this.Property(t => t.UnitName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Goods");
            this.Property(t => t.GoodsId).HasColumnName("GoodsId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.GoodsNo).HasColumnName("GoodsNo");
            this.Property(t => t.GoodsImage).HasColumnName("GoodsImage");
            this.Property(t => t.GoodsName).HasColumnName("GoodsName");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.UnitName).HasColumnName("UnitName");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
        }
    }
}
