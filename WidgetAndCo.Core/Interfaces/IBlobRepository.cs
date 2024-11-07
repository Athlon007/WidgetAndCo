namespace WidgetAndCo.Core.Interfaces;

public interface IBlobRepository
{
    Task UploadBlobAsync(string blobName, Stream content);
    Task DeleteBlobAsync(string blobName);
    Task<Uri> GenerateSasTokenAsync(string blobName);
}