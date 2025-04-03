using Domain.Entities;
using Services.Models;

namespace Services.Interfaces;
public interface ITemplateServices
{
    Task<bool> CreateTemplateAsync(TemplateModel model);
    Task<Template?> GetTemplateAsync(string id);
    Task<bool> DeleteTemplateAsync(string id);
}
