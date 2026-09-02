using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services;

public class MenuService
{
private readonly TableClient _tableClient;


public MenuService()
{
    var connectionString =
        Environment.GetEnvironmentVariable("AzureWebJobsStorage")
        ?? "UseDevelopmentStorage=true";

    _tableClient = new TableClient(connectionString, "MenuItems");

    _tableClient.CreateIfNotExists();
}

public async Task<MenuItem> CreateAsync(MenuItem item)
{
    await _tableClient.AddEntityAsync(item);
    return item;
}

public async Task<List<MenuItem>> GetAllAsync()
{
    var items = new List<MenuItem>();

    await foreach (var item in _tableClient.QueryAsync<MenuItem>())
    {
        items.Add(item);
    }

    return items;
}

public async Task<List<MenuItem>> GetByCategoryAsync(string category)
{
    var items = new List<MenuItem>();

    await foreach (var item in _tableClient.QueryAsync<MenuItem>(
        x => x.PartitionKey == category))
    {
        items.Add(item);
    }

    return items;
}

public async Task<MenuItem?> GetAsync(string category, string id)
{
    try
    {
        var response = await _tableClient.GetEntityAsync<MenuItem>(
            category,
            id);

        return response.Value;
    }
    catch (Azure.RequestFailedException ex)
        when (ex.Status == 404)
    {
        return null;
    }
}

public async Task UpdateAsync(
    string category,
    string id,
    double price,
    bool isAvailable)
{
    var item = await GetAsync(category, id);

    if (item == null)
    {
        throw new KeyNotFoundException("Menu item not found.");
    }

    item.Price = price;
    item.IsAvailable = isAvailable;

    await _tableClient.UpdateEntityAsync(
        item,
        item.ETag,
        TableUpdateMode.Replace);
}

public async Task DeleteAsync(string category, string id)
{
    await _tableClient.DeleteEntityAsync(category, id);
}


}
