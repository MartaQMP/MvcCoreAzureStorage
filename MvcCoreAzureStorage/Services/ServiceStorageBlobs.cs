using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //METODO PARA RECUPERAR TODOS LOS CONTAINERS
        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach(BlobContainerItem container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        //METODO PARA CREAR UN CONTAINER
        public async Task CreateContainerAsync(string containerName)
        {
            await this.client.CreateBlobContainerAsync(containerName.ToLower(), PublicAccessType.None);
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        // LISTADO DE FICHEROS DENTRO DE UN CONTAINER
        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            /* NECESITAMOS UN CLIENTE DE BLOBS CONTAINER PARA EL ACCESO 
             * A LOS FICHEROS */
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();
            await foreach(BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                
                BlobModel blob = new BlobModel
                {
                    Nombre = item.Name,
                    Container = containerName,
                    Url = blobClient.Uri.AbsoluteUri
                };
                models.Add(blob);
            }
            return models;
        }

        // LEER UN BLOB COMO STREAM
        public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobDownloadInfo response = await blobClient.DownloadAsync();
            return response.Content;
        }

        // ELIMINAR UN BLOB
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient = this.client.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteBlobAsync(blobName);
        }

        // SUBIR UN BLOB A UN CONTAINER
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient blobContainerClient = this.client.GetBlobContainerClient(containerName);
            await blobContainerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
