using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Oficios.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.DataAccess.EntityConfiguration
{
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.HasOne(w => w.User)
                   .WithOne()
                   .HasForeignKey<Worker>(w => w.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(w => w.UserId)
                   .IsUnique();
        }
    }
}

