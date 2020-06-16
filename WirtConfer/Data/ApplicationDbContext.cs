using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WirtConfer.Models;

namespace WirtConfer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Event_> Events { get; set; } 
        public DbSet<Invite> Invites { get; set; } 
        public DbSet<Room> Rooms { get; set; } 
        public new DbSet<User> Users { get; set; } 
        public DbSet<UserInEvent> UserInEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event_>()
                .HasMany(o => o.Rooms)
                .WithOne(o => o.Event)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Event_>()
                .HasMany(o => o.Invites)
                .WithOne(o => o.Event)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Event_>()
                .HasMany(o => o.UsersInEvents)
                .WithOne(o => o.Event)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(o => o.UsersInEvents)
                .WithOne(o => o.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
