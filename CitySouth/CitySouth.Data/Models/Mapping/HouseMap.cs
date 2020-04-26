using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class HouseMap : EntityTypeConfiguration<House>
    {
        public HouseMap()
        {
            // Primary Key
            this.HasKey(t => t.HouseId);

            // Properties
            this.Property(t => t.HouseType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.HouseNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Model)
                .HasMaxLength(50);

            this.Property(t => t.Structure)
                .HasMaxLength(50);

            this.Property(t => t.ContactTel)
                .HasMaxLength(50);

            this.Property(t => t.ElseTel)
                .HasMaxLength(50);

            this.Property(t => t.News)
                .HasMaxLength(250);

            this.Property(t => t.Remark)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("House");
            this.Property(t => t.HouseId).HasColumnName("HouseId");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.HouseType).HasColumnName("HouseType");
            this.Property(t => t.Building).HasColumnName("Building");
            this.Property(t => t.Unit).HasColumnName("Unit");
            this.Property(t => t.Floor).HasColumnName("Floor");
            this.Property(t => t.No).HasColumnName("No");
            this.Property(t => t.HouseNo).HasColumnName("HouseNo");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.Structure).HasColumnName("Structure");
            this.Property(t => t.Floorage).HasColumnName("Floorage");
            this.Property(t => t.ContactTel).HasColumnName("ContactTel");
            this.Property(t => t.ElseTel).HasColumnName("ElseTel");
            this.Property(t => t.News).HasColumnName("News");
            this.Property(t => t.IsPlace).HasColumnName("IsPlace");
            this.Property(t => t.IsHasOwner).HasColumnName("IsHasOwner");
            this.Property(t => t.HandDate).HasColumnName("HandDate");
            this.Property(t => t.EmptyState).HasColumnName("EmptyState");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
