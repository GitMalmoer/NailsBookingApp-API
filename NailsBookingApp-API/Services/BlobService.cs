using Azure.Storage.Blobs;

namespace NailsBookingApp_API.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobClient;

        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }



    }
}
