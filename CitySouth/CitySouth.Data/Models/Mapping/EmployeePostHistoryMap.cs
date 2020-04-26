using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class EmployeePostHistoryMap : EntityTypeConfiguration<EmployeePostHistory>
    {
        public EmployeePostHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.HistoryId);

            // Properties
            this.Property(t => t.Transactor)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("EmployeePostHistory");
            this.Property(t => t.HistoryId).HasColumnName("HistoryId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.CurrentPostId).HasColumnName("CurrentPostId");
            this.Property(t => t.ChangePostId).HasColumnName("ChangePostId");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.ChangeDate).HasColumnName("ChangeDate");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Transactor).HasColumnName("Transactor");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
