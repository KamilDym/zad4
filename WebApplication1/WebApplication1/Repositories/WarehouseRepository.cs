using System.Data.SqlClient;

namespace WebApplication1.Repositories;

public interface IWarehouseRepository
{
    Task<int?> AddProductToWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);
}

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int?> AddProductToWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            return null;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }
}