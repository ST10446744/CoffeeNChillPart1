using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class ListStaffDocuments
{
    private readonly DocumentService _documentService;

    public ListStaffDocuments(DocumentService documentService)
    {
        _documentService = documentService;
    }

    [Function("ListStaffDocuments")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents")]
        HttpRequestData req)
    {
        try
        {
            var files = await _documentService.ListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(files);

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