﻿using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace GraphicsCardProject.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<Brand>? Brands { get; set; }
        public virtual DbSet<Category>? Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderLineItem> OrderLineItems { get; set; }
    }
}
