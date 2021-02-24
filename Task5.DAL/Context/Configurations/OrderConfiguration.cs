using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Task5.DAL.Entities;

namespace Task5.DAL.Context.Configurations
{
    public class OrderConfiguration : EntityTypeConfiguration<Order>
    {
        public OrderConfiguration()
        {
            this.ToTable("Orders")
                .HasKey(x => x.Id);

            this.Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Date)
                .IsRequired();

            this.HasRequired(x => x.Client)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.ClientId);

            this.HasRequired(x => x.Product)
               .WithMany(x => x.Orders)
               .HasForeignKey(x => x.ProductId);
        }
    }
}
