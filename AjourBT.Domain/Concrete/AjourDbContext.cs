﻿using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AjourBT.Domain.Abstract;



namespace AjourBT.Domain.Concrete
{
    public class AjourDbContext : DbContext
    {
        public AjourDbContext()
            : base("AjourBTConnection")
        {
            string DBInitType = System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"].ToString();
            switch (DBInitType)
            {
                case ("InitForTest"):
                    Database.SetInitializer(new DropCreateDatabaseAlways<AjourDbContext>());
                    break;
                case ("InitDbClear"):
                    Database.SetInitializer(new DropCreateDatabaseAlways<AjourDbContext>());
                    break;
                default:
                    Database.SetInitializer(new CreateDatabaseIfNotExists<AjourDbContext>());
                    break;
            }
        }

        public AjourDbContext(string connectionString)
            : base(connectionString)
        {
            string DBInitType = System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"].ToString();
            switch (DBInitType)
            {
                case ("InitForTest"):
                    Database.SetInitializer(new DropCreateDatabaseAlways<AjourDbContext>());
                    break;
                case ("InitDbClear"):
                    Database.SetInitializer(new DropCreateDatabaseAlways<AjourDbContext>());
                    break;
                default:
                    Database.SetInitializer(new CreateDatabaseIfNotExists<AjourDbContext>());
                    break;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BusinessTrip>()
                .HasRequired(u => u.Location)
                .WithMany(b => b.BusinessTrips)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
               .HasOptional(u => u.Department)
               .WithMany(b => b.Employees)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<BusinessTrip>()
              .HasRequired(u => u.BTof)
              .WithMany(b => b.BusinessTrips)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
              .HasOptional(v => v.Visa)
              .WithRequired(e => e.VisaOf);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Visa> Visas { get; set; }
        public DbSet<VisaRegistrationDate> VisaRegistrationDates { get; set; }
        public DbSet<BusinessTrip> BusinessTrips { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Permit> Permits { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<PrivateTrip> PrivateTrips { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Journey> Journeys { get; set; }
        public DbSet<CalendarItem> CalendarItems { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<Sickness> Sicknesses { get; set; }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    } 
    
}