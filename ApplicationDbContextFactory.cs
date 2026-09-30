using EduCore.CorePlatform.Application.Abstractions.MultiTenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace EduCore.CorePlatform.Infrastructure.Persistence.DbContext;

/// <summary>
/// Design-time factory for EF Core migrations
/// Factory cho EF Core khi chạy migration
/// </summary>
public sealed class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        // ⚠️ DESIGN - TIME ONLY
        // Không có HttpContext → không có tenant
        ICurrentTenant currentTenant = new DesignTimeTenant();

        return new ApplicationDbContext(optionsBuilder.Options, currentTenant);
    }
    private sealed class DesignTimeTenant : ICurrentTenant
    {
        public Guid? TenantId => null;
    }
}
