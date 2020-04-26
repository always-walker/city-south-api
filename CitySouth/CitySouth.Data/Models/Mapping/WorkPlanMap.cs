using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class WorkPlanMap : EntityTypeConfiguration<WorkPlan>
    {
        public WorkPlanMap()
        {
            // Primary Key
            this.HasKey(t => t.PlanId);

            // Properties
            // Table & Column Mappings
            this.ToTable("WorkPlan");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.WorkDate).HasColumnName("WorkDate");
            this.Property(t => t.IsWork).HasColumnName("IsWork");
        }
    }
}
