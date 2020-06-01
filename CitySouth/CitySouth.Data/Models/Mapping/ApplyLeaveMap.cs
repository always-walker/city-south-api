using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class ApplyLeaveMap : EntityTypeConfiguration<ApplyLeave>
    {
        public ApplyLeaveMap()
        {
            // Primary Key
            this.HasKey(t => t.LeaveId);

            // Properties
            this.Property(t => t.LeaveType)
                .HasMaxLength(50);

            this.Property(t => t.Reason)
                .HasMaxLength(250);

            this.Property(t => t.Image)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("ApplyLeave");
            this.Property(t => t.LeaveId).HasColumnName("LeaveId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.LeaveType).HasColumnName("LeaveType");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.Days).HasColumnName("Days");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.IsPass).HasColumnName("IsPass");
            this.Property(t => t.PassEmployeeId).HasColumnName("PassEmployeeId");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
    }
}
