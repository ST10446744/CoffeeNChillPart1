using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class CreateMenuItem
{
    private readonly MenuService _menuService;

    public CreateMenuItem(MenuService menuService)
    {
        _menuService = menuService;
    }

    [Function("CreateMenuItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")]
        HttpRequestData req)
    {
        try
        {
            var item = await JsonSerializer.DeserializeAsync<MenuItem>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (item == null ||
                string.IsNullOrWhiteSpace(item.PartitionKey) ||
                string.IsNullOrWhiteSpace(item.RowKey) ||
                string.IsNullOrWhiteSpace(item.Name))
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync(
                    "Category, ID and Name are required.");
                return badRequest;
            }

            await _menuService.CreateAsync(item);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(item);

            return response;
        }
        catch (Exception ex)
        {
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync(ex.Message);
            return response;
        }
    }
}