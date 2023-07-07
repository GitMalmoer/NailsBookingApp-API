using Domain.Models;

namespace Application.Services;

public interface IBlobService
{
    Task<string> GetBlob(string blobName, string containerName);
    Task<IEnumerable<AvatarPicture>> ListAvatars(string containerName);
}