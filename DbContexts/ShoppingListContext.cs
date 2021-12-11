using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.DbContexts
{
    public class ShoppingListContext : DbContext
    {
        public ShoppingListContext(DbContextOptions<ShoppingListContext> options) : base (options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingList> Lists { get; set; }
        public DbSet<ProductsList> ProductsList { get; set; }
        public DbSet<Tag> Tags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductsList>()
                .HasKey(pl => new { pl.ProductId, pl.ShoppingListId });
            modelBuilder.Entity<ProductsList>()
                .HasOne(pl => pl.Product)
                .WithMany(p => p.ProductsList)
                .HasForeignKey(pl => pl.ProductId);
            modelBuilder.Entity<ProductsList>()
                .HasOne(pl => pl.ShoppingList)
                .WithMany(s => s.ProductsList)
                .HasForeignKey(pl => pl.ShoppingListId);
            modelBuilder.Entity<ShoppingList>()
                .HasOne(sl => sl.ListTag)
                .WithMany(t => t.ShoppingLists)
                .HasForeignKey(sl => sl.ListTagId);
        }
    }
}
