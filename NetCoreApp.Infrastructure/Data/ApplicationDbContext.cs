using Microsoft.EntityFrameworkCore;
using NetCoreApp.Domain.Entities;
using System.Collections.Generic;
using NetCoreApi.Crosscutting.Helpers;

namespace NetCoreApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<FamilyRole> FamilyRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UsersAddresses { get; set; }
        public DbSet<UserRelationship> UserRelationships { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            BuildAddressModel(modelBuilder);
            BuildUserModel(modelBuilder);
            BuildUserAddressModel(modelBuilder);
            BuildFamilyRoleModel(modelBuilder);
            BuildUserRelationshipModel(modelBuilder);
            AddInitialDataSets(modelBuilder);
        }

        private static void BuildAddressModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
               .Property(u => u.Name)
               .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(u => u.Number)
                .IsRequired();
        }

        private static void BuildUserModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.HeightCm)
                .IsRequired();
        }

        private static void BuildUserAddressModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAddress>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserAddress>()
                .HasOne(ua => ua.Address)
                .WithMany()
                .HasForeignKey(ua => ua.AddressId);

            modelBuilder.Entity<UserAddress>()
                .HasIndex(ua => new { ua.UserId, ua.AddressId }).IsUnique();

        }

        private static void BuildFamilyRoleModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FamilyRole>()
                .Property(fr => fr.Code)
                .IsRequired();

            modelBuilder.Entity<FamilyRole>()
                .HasIndex(fr => fr.Code)
                .IsUnique();

            modelBuilder.Entity<FamilyRole>()
                .Property(fr => fr.Description)
                .IsRequired();
        }

        private static void BuildUserRelationshipModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRelationship>()
                .HasOne(ur => ur.Family)
                .WithMany(f => f.Members)
                .HasForeignKey(ur => ur.FamilyId);

            modelBuilder.Entity<UserRelationship>()
                .HasOne(ur => ur.User)
                .WithMany(f => f.Relationships)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRelationship>()
                .HasOne(ur => ur.FamilyRole)
                .WithMany()
                .HasForeignKey(ur => ur.FamilyRoleId);

            modelBuilder.Entity<UserRelationship>()
                .HasIndex(ur => new { ur.FamilyId, ur.UserId }).IsUnique();
        }

        private void AddInitialDataSets(ModelBuilder modelBuilder)
        {
            InitialDataFamilyRoles(modelBuilder);
        }

        private static void InitialDataFamilyRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FamilyRole>().HasData(new List<FamilyRole>()
            {
                new FamilyRole()
                {
                    Id = (long)NetCoreApi.Crosscutting.Enums.FamilyRoles.Father,
                    Code = NetCoreApi.Crosscutting.Enums.FamilyRoles.Father.GetDescription(),
                    Description = NetCoreApi.Crosscutting.Enums.FamilyRoles.Father.GetDescription()
                },
                new FamilyRole()
                {
                    Id = (long)NetCoreApi.Crosscutting.Enums.FamilyRoles.Mother,
                    Code = NetCoreApi.Crosscutting.Enums.FamilyRoles.Mother.GetDescription(),
                    Description = NetCoreApi.Crosscutting.Enums.FamilyRoles.Father.GetDescription()
                },
                new FamilyRole()
                {
                    Id = (long)NetCoreApi.Crosscutting.Enums.FamilyRoles.Son,
                    Code = NetCoreApi.Crosscutting.Enums.FamilyRoles.Son.GetDescription(),
                    Description = NetCoreApi.Crosscutting.Enums.FamilyRoles.Son.GetDescription()
                },
                new FamilyRole()
                {
                    Id = (long)NetCoreApi.Crosscutting.Enums.FamilyRoles.Daughter,
                    Code = NetCoreApi.Crosscutting.Enums.FamilyRoles.Daughter.GetDescription(),
                    Description = NetCoreApi.Crosscutting.Enums.FamilyRoles.Daughter.GetDescription()
                },
            });
        }
    }
}
