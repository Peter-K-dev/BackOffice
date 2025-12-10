using BackOffice.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BackOffice.Database;

public class MemoryDbContext(
	DbContextOptions<MemoryDbContext> options)
	: DbContext(options)
{
	public DbSet<DomainRule> DomainRules { get; set; }
	
	public DbSet<ChangeLog> ChangeLogs { get; set; }

	public DbSet<Notification> Notifications { get; set; }
}