using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class EstateMap : EntityTypeConfiguration<Estate>
    {
        public EstateMap()
        {
            // Primary Key
            this.HasKey(t => t.EstateId);

            // Properties
            this.Property(t => t.EstateName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .HasMaxLength(250);

            this.Property(t => t.Introduct)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Estate");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.EstateName).HasColumnName("EstateName");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Introduct).HasColumnName("Introduct");
        }
    }
}
