
using BackOffice.Entities;

namespace BackOffice.Repositories;

public interface IDomainRuleRepository
{
	public Task<DomainRule?> GetByIdAsync(int id);
	public Task<DomainRule?> GetByNameAsync(string name);

	public Task<DomainRule> CreateAsync(DomainRule domainRule);

	public Task<DomainRule> UpdateAsync(DomainRule domainRule);

	public Task DeleteAsync(DomainRule domainRule);
	public Task<List<DomainRule>> GetByQueryAsync(DomainRuleQueryModel domainRuleQueryModel);
}