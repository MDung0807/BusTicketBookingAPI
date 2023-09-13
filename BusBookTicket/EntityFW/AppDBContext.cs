﻿
using BusBookTicket.Auth.Models;
using BusBookTicket.Auth.Models.ConfigEntity;
using BusBookTicket.CompanyManage.Models;
using BusBookTicket.CustomerManage.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookTicket.EntityFW
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AccountConfigs());

        }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusType> BusesType { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Account> accounts { get; set; }
    }
}
