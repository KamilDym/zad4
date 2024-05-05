using WebApplication1.DTOs;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public interface IWarehouseService
{
    Task<int> AddProductToWarehouseAsync(WarehouseDto dto);
}

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<int> AddProductToWarehouseAsync(WarehouseDto dto)
    {
        var idProductWarehouse = await _warehouseRepository.AddProductToWarehouseAsync(
            idWarehouse: dto.IdWarehouse!.Value,
            idProduct: dto.IdProduct!.Value,
            idOrder: idOrder,
            createdAt: DateTime.UtcNow);
        
        if (!idProductWarehouse.HasValue)
            throw new Exception("Failed to register product in warehouse");

        return idProductWarehouse.Value;
    }

}