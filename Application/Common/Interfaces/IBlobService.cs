using Domain.Models;

namespace Application.Common.Interfaces;

public interface IBlobService
{
    Task<string> GetBlob(string blobName, string containerName);
    Task<IEnumerable<AvatarPicture>> ListAvatars(string containerName);
}