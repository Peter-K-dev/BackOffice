using BackOffice.Database;
using Microsoft.EntityFrameworkCore;
using BackOffice.Entities;


namespace BackOffice.Repositories;

public class DomainRuleRepository : IDomainRuleRepository
{
	private readonly ILogger<DomainRuleRepository> _logger;
	private readonly MemoryDbContext _memoryDbContext;

	public DomainRuleRepository(ILogger<DomainRuleRepository> logger,
		MemoryDbContext memoryDbContext)
	{
		_logger = logger;
		_memoryDbContext = memoryDbContext;
	}

	public async Task<DomainRule?> GetByIdAsync(int id)
	{
		return await _memoryDbContext.DomainRules.FirstOrDefaultAsync(r=>r.Id == id);

	}

	public async Task<DomainRule?> GetByNameAsync(string name)
	{
		return await _memoryDbContext.DomainRules.FirstOrDefaultAsync(r => r.Name == name);

	}

	public async Task<DomainRule> CreateAsync(DomainRule domainRule)
	{
		_memoryDbContext.DomainRules.Add(domainRule);
		
		await _memoryDbContext.SaveChangesAsync();

		return domainRule;

	}

	public async Task<DomainRule> UpdateAsync(DomainRule domainRule)
	{

		_memoryDbContext.DomainRules.Update(domainRule);

		await _memoryDbContext.SaveChangesAsync();

		return domainRule;
	}

	public async Task DeleteAsync(DomainRule domainRule)
	{
		_memoryDbContext.DomainRules.Remove(domainRule);

		await _memoryDbContext.SaveChangesAsync();
	}

	public async Task<List<DomainRule>> GetByQueryAsync(DomainRuleQueryModel domainRuleQueryModel)
	{
		return await _memoryDbContext.DomainRules.AsNoTracking().Skip(domainRuleQueryModel.Offset * domainRuleQueryModel.Limit).Take(domainRuleQueryModel.Limit - 1).ToListAsync();
	}
}