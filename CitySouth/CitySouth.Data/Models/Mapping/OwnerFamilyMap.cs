using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class OwnerFamilyMap : EntityTypeConfiguration<OwnerFamily>
    {
        public OwnerFamilyMap()
        {
            // Primary Key
            this.HasKey(t => t.PeopleId);

            // Properties
            this.Property(t => t.PeopleName)
                .HasMaxLength(50);

            this.Property(t => t.Sex)
                .HasMaxLength(50);

            this.Property(t => t.Relation)
                .HasMaxLength(50);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("OwnerFamily");
            this.Property(t => t.PeopleId).HasColumnName("PeopleId");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.PeopleName).HasColumnName("PeopleName");
            this.Property(t => t.Sex).HasColumnName("Sex");
            this.Property(t => t.Relation).HasColumnName("Relation");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
