using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodsCategoryMap : EntityTypeConfiguration<GoodsCategory>
    {
        public GoodsCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryId);

            // Properties
            this.Property(t => t.CategoryName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GoodsCategory");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.ParentCategoryId).HasColumnName("ParentCategoryId");
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");
        }
    }
}
