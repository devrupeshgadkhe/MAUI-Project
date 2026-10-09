using InventoryApp.Models;
using SQLite;

namespace InventoryApp.Data;

public sealed class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _initialized;

    public DatabaseService()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "inventory.db3");
        _database = new SQLiteAsyncConnection(path);
    }

    private async Task InitializeAsync()
    {
        if (_initialized) return;
        await _database.CreateTableAsync<Product>();
        _initialized = true;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        await InitializeAsync();
        return await _database.Table<Product>().OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<int> SaveProductAsync(Product product)
    {
        await InitializeAsync();
        return product.Id == 0
            ? await _database.InsertAsync(product)
            : await _database.UpdateAsync(product);
    }

    public async Task<int> DeleteProductAsync(int id)
    {
        await InitializeAsync();
        return await _database.DeleteAsync<Product>(id);
    }
}
