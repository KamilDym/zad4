using WebApplication1.Repositories;

namespace WebApplication1.Services;

public interface IWarehouseService
{
    
}

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

}