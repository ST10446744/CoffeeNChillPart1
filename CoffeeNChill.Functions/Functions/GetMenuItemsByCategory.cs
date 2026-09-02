using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class GetMenuItemsByCategory
{
    private readonly MenuService _menuService;

    public GetMenuItemsByCategory(MenuService menuService)
    {
        _menuService = menuService;
    }

    [Function("GetMenuItemsByCategory")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "menu/category/{category}")]
        HttpRequestData req,
        string category)
    {
        try
        {
            var items = await _menuService.GetByCategoryAsync(category);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);

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
}