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

    private async Task CheckIfProductExist(int idProduct)
    {
        if(!await _warehouseRepository.CheckIfProductExist(idProduct))
            throw new Exception("No product found on given id");
    }

    private async Task CheckIfWarehouseExist(int idWarehouse)
    {
        if (!await _warehouseRepository.CheckIfWarehouseExist(idWarehouse))
            throw new Exception("No warehouse found on given id");
    }

    private  void CheckAmount(int amount)
    {
        if (amount <= 0)
            throw new Exception("Amount must be higher that 0");
    }

    private async Task CheckIfOrderExist(int idProduct,int amount,DateTime reqestDate)
    {
        if(!await _warehouseRepository.CheckIfOrderExist(idProduct, amount, reqestDate))
            throw new Exception("No matching order found");
    }

    private async Task CheckIfFulfilled(int idOrder)
    {
        if(!await _warehouseRepository.CheckIfFulfilled(idOrder))
            throw new Exception("Order already fulfilled");
    }


    public async Task<int> AddProductToWarehouseAsync(WarehouseDto dto)
    {
        const int idOrder = 1;

        await CheckIfProductExist(dto.IdProduct!.Value);
        await CheckIfWarehouseExist(dto.IdWarehouse!.Value);
        CheckAmount(dto.Amount!.Value);
        await CheckIfOrderExist(dto.IdProduct!.Value, dto.Amount!.Value, dto.CreatedAt!.Value);
        await CheckIfFulfilled(idOrder);
        
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