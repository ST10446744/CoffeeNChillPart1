using Azure.Storage.Files.Shares;

namespace CoffeeNChill.Functions.Services;

public class DocumentService
{
    private readonly ShareClient _shareClient;
    private readonly ShareDirectoryClient _directoryClient;

    private const string ShareName = "staff-docs";

    public DocumentService()
    {
        var connectionString =
            Environment.GetEnvironmentVariable("AzureFilesConnectionString")
            ?? throw new InvalidOperationException(
                "AzureFilesConnectionString is not configured.");

        _shareClient = new ShareClient(connectionString, ShareName);

        _shareClient.CreateIfNotExists();

        _directoryClient = _shareClient.GetRootDirectoryClient();
    }

    public async Task UploadAsync(string fileName, Stream content)
    {
        var fileClient = _directoryClient.GetFileClient(fileName);

        await fileClient.CreateAsync(content.Length);

        await fileClient.UploadAsync(content);
    }

    public async Task<List<StaffDocumentInfo>> ListAsync()
    {
        var files = new List<StaffDocumentInfo>();

        await foreach (var item in _directoryClient.GetFilesAndDirectoriesAsync())
        {
            if (!item.IsDirectory)
            {
                var fileClient = _directoryClient.GetFileClient(item.Name);

                var properties = await fileClient.GetPropertiesAsync();

                files.Add(new StaffDocumentInfo
                {
                    FileName = item.Name,
                    Size = properties.Value.ContentLength,
                    LastModified = properties.Value.LastModified
                });
            }
        }

        return files;
    }

    public async Task<(Stream Content, string ContentType, long Size)?> DownloadAsync(
        string fileName)
    {
        var fileClient = _directoryClient.GetFileClient(fileName);

        if (!await fileClient.ExistsAsync())
        {
            return null;
        }

        var download = await fileClient.DownloadAsync();

        var memoryStream = new MemoryStream();

        await download.Value.Content.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        var contentType = "application/octet-stream";

if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
{
    contentType = "application/pdf";
}

return (
    memoryStream,
    contentType,
    memoryStream.Length
);
    }

    public class StaffDocumentInfo
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTimeOffset LastModified { get; set; }
    }
}