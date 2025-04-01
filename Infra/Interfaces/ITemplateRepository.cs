using Domain.Entities;

namespace Infra.Interfaces;
public interface ITemplateRepository : IRepository<Template>
{
    Task<List<Template>> GetAllAsync();
}
