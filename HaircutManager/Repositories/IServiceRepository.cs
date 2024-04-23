using HaircutManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IServiceRepository
{
    Task<List<Service>> ListAsync();
    Task<Service> FindByIdAsync(int id);
    Task AddAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(Service service);
    bool Exists(int id);
}
