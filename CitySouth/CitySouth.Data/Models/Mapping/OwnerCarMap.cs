using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class OwnerCarMap : EntityTypeConfiguration<OwnerCar>
    {
        public OwnerCarMap()
        {
            // Primary Key
            this.HasKey(t => t.CarId);

            // Properties
            this.Property(t => t.UserName)
                .HasMaxLength(50);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.Brand)
                .HasMaxLength(50);

            this.Property(t => t.Model)
                .HasMaxLength(50);

            this.Property(t => t.CarNumber)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("OwnerCar");
            this.Property(t => t.CarId).HasColumnName("CarId");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Brand).HasColumnName("Brand");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.CarNumber).HasColumnName("CarNumber");
            this.Property(t => t.ParkingExpireDate).HasColumnName("ParkingExpireDate");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
