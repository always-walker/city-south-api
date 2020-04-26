using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class HandHouseMap : EntityTypeConfiguration<HandHouse>
    {
        public HandHouseMap()
        {
            // Primary Key
            this.HasKey(t => t.HandId);

            // Properties
            this.Property(t => t.Remark)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("HandHouse");
            this.Property(t => t.HandId).HasColumnName("HandId");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.HandDate).HasColumnName("HandDate");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.PropertyStartDate).HasColumnName("PropertyStartDate");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
