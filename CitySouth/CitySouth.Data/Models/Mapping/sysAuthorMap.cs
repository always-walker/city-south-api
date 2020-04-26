using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class sysAuthorMap : EntityTypeConfiguration<sysAuthor>
    {
        public sysAuthorMap()
        {
            // Primary Key
            this.HasKey(t => t.AuthorId);

            // Properties
            this.Property(t => t.AuthorName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MenuLink)
                .HasMaxLength(250);

            this.Property(t => t.AuthorKey)
                .HasMaxLength(250);

            this.Property(t => t.MenuIcon)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("sysAuthor");
            this.Property(t => t.AuthorId).HasColumnName("AuthorId");
            this.Property(t => t.ParentAuthorId).HasColumnName("ParentAuthorId");
            this.Property(t => t.AuthorName).HasColumnName("AuthorName");
            this.Property(t => t.MenuLink).HasColumnName("MenuLink");
            this.Property(t => t.AuthorKey).HasColumnName("AuthorKey");
            this.Property(t => t.MenuIcon).HasColumnName("MenuIcon");
            this.Property(t => t.Sort).HasColumnName("Sort");
        }
    }
}
