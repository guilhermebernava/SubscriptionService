using Domain.Entities;

namespace Services.Interfaces;
public interface ITemplateServices
{
    Task<bool> CreateTemplateAsync(string customTemplate);
    Task<Template?> GetTemplateAsync(string id);
    Task<bool> DeleteTemplateAsync(string id);
}
