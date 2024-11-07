using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class BlobRepository(IConfiguration configuration) : IBlobRepository
{
    private readonly BlobContainerClient _blobContainerClient = new(configuration["ConnectionStrings:BlobStorageConnection"], "images");

    public async Task UploadBlobAsync(string blobName, Stream content)
    {
        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        // Create container if it doesn't exist
        await _blobContainerClient.CreateIfNotExistsAsync();

        await blobClient.UploadAsync(content);
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<Uri> GenerateSasTokenAsync(string blobName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(blobName);
        var builder = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
        return blobClient.GenerateSasUri(builder);
    }
}