using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficiosEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.DataAccess.EntityConfiguration
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasOne(c => c.User)
                   .WithOne()
                   .HasForeignKey<Client>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.UserId)
                   .IsUnique();
        }
    }
}

