using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class DownloadStaffDocument
{
    private readonly DocumentService _documentService;

    public DownloadStaffDocument(DocumentService documentService)
    {
        _documentService = documentService;
    }

    [Function("DownloadStaffDocument")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents/download/{fileName}")]
        HttpRequestData req,
        string fileName)
    {
        try
        {
            var result = await _documentService.DownloadAsync(fileName);

            if (result == null)
            {
                var notFound = req.CreateResponse(
                    HttpStatusCode.NotFound);

                await notFound.WriteStringAsync(
                    "Staff document not found.");

                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add(
                "Content-Type",
                result.Value.ContentType);

            response.Headers.Add(
                "Content-Disposition",
                $"attachment; filename=\"{fileName}\"");

            response.Headers.Add(
                "Content-Length",
                result.Value.Size.ToString());

            await result.Value.Content.CopyToAsync(
                response.Body);

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