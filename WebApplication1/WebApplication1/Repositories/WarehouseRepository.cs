using System.Data.SqlClient;

namespace WebApplication1.Repositories;

public interface IWarehouseRepository
{
    Task<int?> AddProductToWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);
    Task<bool> CheckIfProductExist(int idProduct);
    Task<bool> CheckIfWarehouseExist(int idWarehouse);
    Task<bool> CheckIfOrderExist(int idProduct,int amount,DateTime reqestDate);
    Task<bool> CheckIfFulfilled(int idOrder);
}

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> CheckIfProductExist(int idProduct)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT * from Product where IdProduct=@IdProduct";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);

        if (await command.ExecuteScalarAsync() is null)
            return false;
        return true;
    }

    public async Task<bool> CheckIfWarehouseExist(int idWarehouse)
    {   
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = @"SELECT * from Warehouse where IdWarehouse = @IdWarehouse";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);

        if (await command.ExecuteScalarAsync() is null)
            return false;
        return true;
    }

    public async Task<bool> CheckIfOrderExist(int idProduct,int amount,DateTime reqestDate)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT * from [Order] WHERE IdProduct = @IdProduct AND Amount=@Amount AND CreatedAt < @RequestDate";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@RequestDate", reqestDate);

        if (await command.ExecuteScalarAsync() is null)
            return false;
        return true;
    }

    public async Task<bool> CheckIfFulfilled(int idOrder)
    {
        
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT * from Product_Warehouse WHERE IdOrder = @IdOrder";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        
        if (await command.ExecuteScalarAsync() is not null)
            return false;
        return true;
    }

    public async Task<int?> AddProductToWarehouseAsync(int idWarehouse, int idProduct, int idOrder, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var query = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
            await using var command = new SqlCommand(query, connection);
            command.Transaction = (SqlTransaction)transaction;
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();
            
            command.CommandText = @"
                      INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, CreatedAt, Amount, Price)
                      OUTPUT Inserted.IdProductWarehouse
                      VALUES (@IdWarehouse, @IdProduct, @IdOrder, @CreatedAt, 0, 0);";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            var idProductWarehouse = (int)await command.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return idProductWarehouse;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }
    
    
}