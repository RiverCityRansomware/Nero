using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace NeroLib {
    public class UsersContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<World> Worlds { get; set; }
        public DbSet<Parse> Parses { get; set; }
        public DbSet<Server> Servers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // TODO: change this to use libconfig when not dev build
            // Data Source={LibConfiguration.Load().ConnectionString}
            optionsBuilder.UseSqlite($"Data Source=E:\\Nero\\NeroLib\\users.db").EnableSensitiveDataLogging(true);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasKey(c => c.UserId);
            modelBuilder.Entity<World>().HasKey(w => w.WorldId);
        }

    }

    // DiscordID is the primary key,
    public class User {
        public ulong UserId  { get; set; }
        public string Username  { get; set; }
        public string Name      { get; set; }
        public int WorldId      { get; set; }
        public World World      { get; set; }
        public ulong ServerId     { get; set; }
        public Server Server    { get; set; }
        public ICollection<Parse> Parses    { get; set; } = new List<Parse>();
    }

    public class Server {
        public ulong ServerId     { get; set; }
        public string Name      { get; set; }
        public int Population   { get; set; }
        public ICollection<User> Users      { get; set; } = new List<User>();
        public ICollection<Parse> Parses    { get; set; } = new List<Parse>();
    }

    public class Parse {
        public string ParseId               { get; set; }
        public ulong UserId                 { get; set; }
        public string Difficulty            { get; set; }
        public string JobName               { get; set; }
        public string JobAbrv               { get; set; }
        public string Size                  { get; set; }
        public string Kill                  { get; set; }
        public string Name                  { get; set; }
        public double PerSecondAmount        { get; set; }
        public double Percent               { get; set; }
        public double HistoricalCount       { get; set; }
        public double HistoricalPercent     { get; set; }
        public ulong ServerId               { get; set; }
    }

    // WorldID is the primary Key
    public class World {
        public int WorldId              { get; set; }
        public string WorldName         { get; set; }
        public string DataCenter        { get; set; }
        public string Region            { get; set; }
        public UInt32 Population        { get; set; }
        public ICollection<User> Users  { get; set; } = new List<User>();
    }

}