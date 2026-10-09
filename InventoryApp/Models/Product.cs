using SQLite;

namespace InventoryApp.Models;

[Table("Products")]
public class Product
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}
