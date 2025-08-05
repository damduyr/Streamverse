
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
    public class ApplicationDbConnection : DbContext
    {
        public ApplicationDbConnection() : base("StreamverseDbConnection") { }
        public DbSet<User> USERS { get; set; }
        public DbSet<UserSubscription> SUBSCRIPTION { get; set; }
        public DbSet<SubscriptionPlan> SUBSCRIPTION_PLAN { get; set; }
        public DbSet<Content> CONTENT { get; set; }
        public DbSet<Movie> MOVIE { get; set; }
        public DbSet<Genre> GENRE { get; set; }
        public DbSet<UserProfile> USER_PROFILE { get; set; }
        public DbSet<Admin> ADMIN { get; set; }
        public DbSet<Watchlist> WATCHLIST { get; set; }
        public DbSet<Review> REVIEW { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("USERS");
            modelBuilder.Entity<UserSubscription>().ToTable("SUBSCRIPTION");
            modelBuilder.Entity<SubscriptionPlan>().ToTable("SUBSCRIPTION_PLAN");
            modelBuilder.Entity<Content>().ToTable("CONTENT");
            modelBuilder.Entity<Movie>().ToTable("MOVIE");
            modelBuilder.Entity<Genre>().ToTable("GENRE");
            modelBuilder.Entity<UserProfile>().ToTable("USER_PROFILE");
            modelBuilder.Entity<Admin>().ToTable("ADMIN");
            modelBuilder.Entity<Watchlist>().ToTable("WATCHLIST");
            modelBuilder.Entity<Review>().ToTable("REVIEW");


        }
    }
}