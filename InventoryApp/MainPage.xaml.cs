using InventoryApp.Data;
using InventoryApp.Models;
using InventoryApp.Services;
using System.Globalization;

namespace InventoryApp;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _database = new();
    private List<Product> _products = new();
    private readonly UpdateService _updateService = new();
    private bool _updateCheckStarted;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshProductsAsync();
        if (!_updateCheckStarted)
        {
            _updateCheckStarted = true;
            _ = _updateService.CheckAndOfferUpdateAsync(this);
        }
    }

    private async Task RefreshProductsAsync()
    {
        _products = await _database.GetProductsAsync();
        ApplySearch();
    }

    private void ApplySearch()
    {
        var search = ProductSearchBar.Text?.Trim() ?? string.Empty;
        var filtered = string.IsNullOrWhiteSpace(search)
            ? _products
            : _products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        ProductsCollection.ItemsSource = filtered;
        StockSummaryLabel.Text = $"{filtered.Count} products · {filtered.Sum(p => p.Quantity)} units";
    }

    private async void AddProduct_Clicked(object sender, EventArgs e)
    {
        var name = ProductNameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Product name required", "Enter a product name.", "OK");
            return;
        }

        if (!int.TryParse(QuantityEntry.Text, out var quantity) || quantity < 0)
        {
            await DisplayAlert("Invalid quantity", "Enter a whole number zero or greater.", "OK");
            return;
        }

        var priceText = (PriceEntry.Text ?? "0").Trim();
        if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.CurrentCulture, out var price) &&
            !decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out price))
        {
            await DisplayAlert("Invalid price", "Enter a valid price.", "OK");
            return;
        }

        if (price < 0)
        {
            await DisplayAlert("Invalid price", "Price cannot be negative.", "OK");
            return;
        }

        await _database.SaveProductAsync(new Product { Name = name, Quantity = quantity, Price = price });
        ProductNameEntry.Text = string.Empty;
        QuantityEntry.Text = string.Empty;
        PriceEntry.Text = string.Empty;
        await RefreshProductsAsync();
    }

    private void ProductSearchBar_TextChanged(object sender, TextChangedEventArgs e) => ApplySearch();

    private async void DeleteProduct_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: not null } button ||
            !int.TryParse(button.CommandParameter.ToString(), out var id))
            return;

        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is null) return;

        var confirm = await DisplayAlert("Delete product", $"Delete '{product.Name}'?", "Delete", "Cancel");
        if (!confirm) return;

        await _database.DeleteProductAsync(id);
        await RefreshProductsAsync();
    }
}
