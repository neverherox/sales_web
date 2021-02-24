using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Task5.DAL.Entities;

namespace Task5.DAL.Context.Configurations
{
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            this.ToTable("Products")
                .HasKey(x => x.Id);           

            this.Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Name)
                .HasMaxLength(30)
                .IsRequired();

            this.Property(x => x.Price)
                .IsRequired();
        }
    }
}
