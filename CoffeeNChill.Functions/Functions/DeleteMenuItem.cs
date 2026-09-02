using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class DeleteMenuItem
{
    private readonly MenuService _menuService;

    public DeleteMenuItem(MenuService menuService)
    {
        _menuService = menuService;
    }

    [Function("DeleteMenuItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "delete",
            Route = "menu/{category}/{id}")]
        HttpRequestData req,
        string category,
        string id)
    {
        try
        {
            var item = await _menuService.GetAsync(category, id);

            if (item == null)
            {
                var notFound = req.CreateResponse(
                    HttpStatusCode.NotFound);

                await notFound.WriteStringAsync(
                    "Menu item not found.");

                return notFound;
            }

            await _menuService.DeleteAsync(category, id);

            var response = req.CreateResponse(
                HttpStatusCode.OK);

            await response.WriteAsJsonAsync(new
            {
                Message = "Menu item deleted successfully.",
                Category = category,
                Id = id
            });

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