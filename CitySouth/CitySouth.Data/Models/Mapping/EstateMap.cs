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

            this.Property(t => t.ImageList)
                .HasMaxLength(500);

            this.Property(t => t.Address)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Estate");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.EstateName).HasColumnName("EstateName");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.BuildingArea).HasColumnName("BuildingArea");
            this.Property(t => t.ResidentialArea).HasColumnName("ResidentialArea");
            this.Property(t => t.ResidentialCount).HasColumnName("ResidentialCount");
            this.Property(t => t.HouseArea).HasColumnName("HouseArea");
            this.Property(t => t.HouseCount).HasColumnName("HouseCount");
            this.Property(t => t.BasementArea).HasColumnName("BasementArea");
            this.Property(t => t.CarplaceCount).HasColumnName("CarplaceCount");
            this.Property(t => t.MatchingArea).HasColumnName("MatchingArea");
            this.Property(t => t.GreenArea).HasColumnName("GreenArea");
            this.Property(t => t.ImageList).HasColumnName("ImageList");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Introduct).HasColumnName("Introduct");
        }
    }
}
