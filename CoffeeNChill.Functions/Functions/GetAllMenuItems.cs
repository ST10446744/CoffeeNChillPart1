using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class GetAllMenuItems
{
    private readonly MenuService _menuService;

    public GetAllMenuItems(MenuService menuService)
    {
        _menuService = menuService;
    }

    [Function("GetAllMenuItems")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")]
        HttpRequestData req)
    {
        try
        {
            var items = await _menuService.GetAllAsync();

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