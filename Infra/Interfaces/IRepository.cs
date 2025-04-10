namespace Infra.Interfaces;
public interface IRepository<T> where T : class
{
    Task<bool> CreateAsync(T subscription);
    Task<T?> GetByIdAsync(string id);
    Task<bool> DeleteAsync(string id);
    Task<bool> UpdateAsync(T subscription); 
}
