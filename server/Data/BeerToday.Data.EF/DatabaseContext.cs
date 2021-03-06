﻿namespace BeerToday.Data.EF
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using Model.Results;
    using Model.Entities;

    using Utilities.Common.Extensions;

    public class DatabaseContext : IdentityDbContext<User, Role, long>, IDatabaseContext
    {
        public DbSet<UserType> UserTypes { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<BusinessSignUpApplication> BusinessSignUpApplications { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<TEntity> DbSet<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        public new EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return base.Entry(entity);
        }

        public CommitResult Commit()
        {
            try
            {
                SaveChanges();

                return new CommitResult(true);
            }
            catch (Exception exception)
            {
                return new CommitResult(exception);
            }
        }

        public async Task<CommitResult> CommitAsync()
        {
            try
            {
                await SaveChangesAsync();

                return new CommitResult(true);
            }
            catch (Exception exception)
            {
                return new CommitResult(exception);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var excecutingAssembly = Assembly.GetExecutingAssembly();

            var contextMappingTypes = excecutingAssembly
                .GetTypes()
                .Where(type => type.ImplementGenericInterface(typeof(IEntityTypeConfiguration<>)))
                .ToArray();

            var applyConfigurationMethod = typeof(ModelBuilder)
                .GetMethods()
                .Where(method => method.Name == nameof(modelBuilder.ApplyConfiguration))
                .First(method => method.GetParameters().Any(parameter => parameter.ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            foreach(var contextMappingType in contextMappingTypes)
            {
                var entityType = contextMappingType.GetGenericInterfaceArguments(typeof(IEntityTypeConfiguration<>))
                    .First();

                var contextMap = Activator.CreateInstance(contextMappingType);

                applyConfigurationMethod.MakeGenericMethod(entityType)
                    .Invoke(modelBuilder, new[] { contextMap });
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}

