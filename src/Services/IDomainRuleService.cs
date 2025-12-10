using BackOffice.Entities;
using BackOffice.Models.DomainRule;


namespace BackOffice.Services;

public interface IDomainRuleService
{
	Task<DomainRule?> GetAsync(int id);

	Task<int> CreateAsync(DomainRule domainRule);

	Task UpdateAsync(DomainRule domainRule);

	Task DeleteAsync(int id);

	Task<List<DomainRule>> GetAsync(DomainRuleQueryModel domainRuleQueryModel);
}