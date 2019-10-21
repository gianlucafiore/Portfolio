using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Db : DbContext
    {
        public DbSet<Usuario> Usuarios{get;set;}
        
        public Db(DbContextOptions<Db> options) : base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}