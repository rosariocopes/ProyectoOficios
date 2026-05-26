using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Oficios.Entities;
using OficiosEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oficios.DataAccess
{
    public class DbDataAccess : IdentityDbContext 
    {
        public virtual DbSet<Client> Clients { get; set; } 
        public virtual DbSet<Worker> Workers { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Proposal> Proposals { get; set; }
        public virtual DbSet<Profession> Professions { get; set; }
        public virtual DbSet <Review> Reviews { get; set; }
        public DbDataAccess(DbContextOptions<DbDataAccess> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.LogTo(Console.WriteLine).EnableDetailedErrors();


    }
}
