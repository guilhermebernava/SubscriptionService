using Domain.Entities;
using Services.Models;

namespace Services.Interfaces;
public interface ITemplateServices
{
    Task<bool> CreateTemplateAsync(TemplateModel model);
    Task<Template?> GetTemplateAsync(string id);
    Task<List<Template>> GetAllTemplatesAsync();
    Task<List<Template>> GetByUserIdTemplatesAsync(string userId);
    Task<bool> UpdateTemplateAsync(TemplateUpdateModel subscription);
    Task<bool> DeleteTemplateAsync(string id);
}
