using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodsOutMap : EntityTypeConfiguration<GoodsOut>
    {
        public GoodsOutMap()
        {
            // Primary Key
            this.HasKey(t => t.OutId);

            // Properties
            this.Property(t => t.OutNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Geter)
                .HasMaxLength(50);

            this.Property(t => t.Outer)
                .HasMaxLength(50);

            this.Property(t => t.Passer)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GoodsOut");
            this.Property(t => t.OutId).HasColumnName("OutId");
            this.Property(t => t.OutNo).HasColumnName("OutNo");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.Geter).HasColumnName("Geter");
            this.Property(t => t.GetDate).HasColumnName("GetDate");
            this.Property(t => t.Outer).HasColumnName("Outer");
            this.Property(t => t.Passer).HasColumnName("Passer");
            this.Property(t => t.PassStatus).HasColumnName("PassStatus");
            this.Property(t => t.PassDate).HasColumnName("PassDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
    }
}
