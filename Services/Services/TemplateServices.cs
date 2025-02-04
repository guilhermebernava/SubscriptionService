using Domain.Entities;
using Infra.Interfaces;
using Services.Interfaces;

namespace Services.Services;
public class TemplateServices : ITemplateServices
{
    private readonly ITemplateRepository _repository;

    public TemplateServices(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateTemplateAsync(string customTemplate) => await _repository.CreateAsync(new Template(customTemplate));

    public async Task<Template?> GetTemplateAsync(string id) => await _repository.GetByIdAsync(id);

    public async Task<bool> DeleteTemplateAsync(string id) => await _repository.DeleteAsync(id);
}
