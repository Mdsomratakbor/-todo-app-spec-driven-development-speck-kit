using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task ReassignTodosToCategoryAsync(Guid fromCategoryId, Guid toCategoryId);
    void Add(Category category);
    void Update(Category category);
    void Delete(Category category);
}
