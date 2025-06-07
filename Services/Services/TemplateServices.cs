using Domain.Entities;
using Infra.Interfaces;
using Services.Interfaces;
using Services.Models;

namespace Services.Services;
public class TemplateServices : ITemplateServices
{
    private readonly ITemplateRepository _repository;

    public TemplateServices(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateTemplateAsync(TemplateModel model) => await _repository.CreateAsync(new Template(model.CustomTemplate,model.UserId));

    public async Task<Template?> GetTemplateAsync(string id) => await _repository.GetByIdAsync(id);

    public async Task<bool> DeleteTemplateAsync(string id) => await _repository.DeleteAsync(id);

    public async Task<List<Template>> GetAllTemplatesAsync() => await _repository.GetAllAsync();

    public async Task<bool> UpdateTemplateAsync(TemplateUpdateModel model)
    {
        if (model.Id == null) throw new ArgumentException("Id could not be null");

        var template = new Template
        {
            Id = model.Id,
            TemplateHtml = model.TemplateHtml,
            UserId = model.UserId,
        };

        return await _repository.UpdateAsync(template);
    }

    public async Task<List<Template>> GetByUserIdTemplatesAsync(string userId) => await _repository.GetByUserIdAsync(userId);

}
