using NailsBookingApp_API.Models;

namespace NailsBookingApp_API.Services;

public interface IBlobService
{
    Task<string> GetBlob(string blobName, string containerName);
    Task<IEnumerable<AvatarPicture>> ListAvatars(string containerName);
}