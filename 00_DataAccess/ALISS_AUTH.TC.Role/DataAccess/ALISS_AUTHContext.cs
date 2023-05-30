using ALISS_AUTH.TC.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ALISS_AUTH.TC.Role.DataAccess
{
    public class ALISS_AUTHContext : DbContext
    {
        private static IConfiguration _iconfiguration;

        public DbSet<TCRole> TCRoles { get; set; }
        public DbSet<TCRolePermission> TCRolePermissions { get; set; }

        //public ALISS_AUTHContext(DbContextOptions<ALISS_AUTHContext> options) : base(options)
        //{

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            optionsBuilder.UseSqlServer(_iconfiguration.GetConnectionString("ALISS_AUTHContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TCRole>().HasKey(x => x.rol_id);
            builder.Entity<TCRole>().ToTable("TCRole");

            builder.Entity<TCRolePermission>().HasKey(x => x.rop_id);
            builder.Entity<TCRolePermission>().ToTable("TCRolePermission");

            base.OnModelCreating(builder);
        }
    }
}
