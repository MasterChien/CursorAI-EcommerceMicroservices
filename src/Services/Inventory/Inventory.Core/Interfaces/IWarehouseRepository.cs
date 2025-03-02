using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<Warehouse> GetByCodeAsync(string code);
        Task<IReadOnlyList<Warehouse>> GetActiveWarehousesAsync();
        Task<Warehouse> GetWarehouseWithItemsAsync(int warehouseId);
    }
} 