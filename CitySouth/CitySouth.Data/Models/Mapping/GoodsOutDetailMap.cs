using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodsOutDetailMap : EntityTypeConfiguration<GoodsOutDetail>
    {
        public GoodsOutDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.DetailId);

            // Properties
            this.Property(t => t.DetailId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("GoodsOutDetail");
            this.Property(t => t.DetailId).HasColumnName("DetailId");
            this.Property(t => t.OutId).HasColumnName("OutId");
            this.Property(t => t.GoodsId).HasColumnName("GoodsId");
            this.Property(t => t.Count).HasColumnName("Count");
        }
    }
}
