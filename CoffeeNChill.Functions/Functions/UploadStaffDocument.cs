using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions.Functions;

public class UploadStaffDocument
{
    private readonly DocumentService _documentService;

    public UploadStaffDocument(DocumentService documentService)
    {
        _documentService = documentService;
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "documents/upload")]
        HttpRequest req)
    {
        try
        {
            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult(
                    "Request must use multipart/form-data.");
            }

            var form = await req.ReadFormAsync();

            if (form.Files.Count == 0)
            {
                return new BadRequestObjectResult(
                    "No file was received.");
            }

            var file = form.Files[0];

            if (file.Length == 0)
            {
                return new BadRequestObjectResult(
                    "The uploaded file is empty.");
            }

            var fileName = Path.GetFileName(file.FileName);

            await using var stream = file.OpenReadStream();

            await _documentService.UploadAsync(
                fileName,
                stream);

            return new CreatedResult(
                $"/api/documents/download/{fileName}",
                new
                {
                    Message = "Staff document uploaded successfully.",
                    FileName = fileName,
                    Size = file.Length
                });
        }
        catch (Exception ex)
        {
            return new ObjectResult(ex.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}