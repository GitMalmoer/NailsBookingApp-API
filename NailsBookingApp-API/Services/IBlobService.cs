namespace NailsBookingApp_API.Services;

public interface IBlobService
{
    Task<string> GetBlob(string blobName, string containerName);
}