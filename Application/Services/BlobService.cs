using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Models;

namespace Application.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;

        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task<string> GetBlob(string blobName, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<IEnumerable<AvatarPicture>> ListAvatars(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            List<AvatarPicture> blobAvatars = new List<AvatarPicture>();
            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                var path = blobClient.Uri.AbsoluteUri;

                AvatarPicture avatarPic = new AvatarPicture()
                {
                    Name = blobClient.Name,
                    Path = path,
                };
                blobAvatars.Add(avatarPic);
            }

            return blobAvatars;
        }

    }
}
