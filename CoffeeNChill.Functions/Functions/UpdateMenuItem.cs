using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class UpdateMenuItem
{
    private readonly MenuService _menuService;

    public UpdateMenuItem(MenuService menuService)
    {
        _menuService = menuService;
    }

    [Function("UpdateMenuItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "put",
            Route = "menu/{category}/{id}")]
        HttpRequestData req,
        string category,
        string id)
    {
        try
        {
            var update = await JsonSerializer.DeserializeAsync<MenuUpdate>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (update == null)
            {
                var badRequest = req.CreateResponse(
                    HttpStatusCode.BadRequest);

                await badRequest.WriteStringAsync(
                    "Price and IsAvailable are required.");

                return badRequest;
            }

            await _menuService.UpdateAsync(
                category,
                id,
                update.Price,
                update.IsAvailable);

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(new
            {
                Message = "Menu item updated successfully.",
                Category = category,
                Id = id
            });

            return response;
        }
        catch (KeyNotFoundException)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);

            await response.WriteStringAsync(
                "Menu item not found.");

            return response;
        }
        catch (Exception ex)
        {
            var response = req.CreateResponse(
                HttpStatusCode.InternalServerError);

            await response.WriteStringAsync(ex.Message);

            return response;
        }
    }

    private class MenuUpdate
    {
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}