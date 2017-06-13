using Amazon;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Cloud
{
    public class StorageService : IStorageService
    {
        private IAppSettingsService _appSettings;

        #region Consts

        private const string AWSAccessKey = "AKIAJHIH2CSOLRXF2YKQ";
        private const string AWSSecretAccessKey = "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";
        private const string AWSBucketName = "busykid-img/Images";
        private const string AWSUrl = "https://busykid-img.s3.amazonaws.com/Images/";

        #endregion

        public StorageService(IAppSettingsService appSettings)
        {
            _appSettings = appSettings;
        }

        /// <summary>
        /// Save file.
        /// </summary>
        /// <param name="fileStream">The file bytes.</param>
        /// <param name="filename">The fime name.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file url.</returns>
        public async Task<string> SaveFile(byte[] fileStream, string filename, string contentType)
        {
            if (_appSettings.IsProduction)
                return await SaveFileToAmazon(fileStream, filename, contentType);
            else
                return await SaveFileToAzure(fileStream, filename, contentType);
        }

        /// <summary>
        /// Delete the file.
        /// </summary>
        /// <param name="imagePath">The image path</param>
        /// <returns>None.</returns>
        public async Task DeleteFile(string imagePath)
        {
            if (_appSettings.IsProduction)
                await DeleteFileFromAmazon(imagePath);
            else
                await DeleteFileFromAzure(imagePath);
        }

        /// <summary>
        /// Gets the blob container
        /// </summary>
        /// <returns>The azure cloud blob container.</returns>
        private CloudBlobContainer GetBlobContainer()
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(_appSettings.AzureConnectionString);

            // Create the blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            var container = blobClient.GetContainerReference("mjc");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            return container;
        }

        /// <summary>
        /// Save file to azure.
        /// </summary>
        /// <param name="fileStream">The file bytes.</param>
        /// <param name="filename">The fime name.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file url.</returns>
        private async Task<string> SaveFileToAzure(byte[] fileStream, string filename, string contentType)
        {
            // Upload image to azure
            var container = GetBlobContainer();

            // Retrieve reference to a blob named "myblob". *** string.Format("{0}/{1}", folderName, fileName)
            var blockBlob = container.GetBlockBlobReference(string.Format("Images/{0}", filename));

            blockBlob.Properties.ContentType = contentType;

            using (var stream = new MemoryStream(fileStream))
            {
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.UploadFromStreamAsync(stream);
            }
            return container.GetBlockBlobReference(string.Format("Images/{0}", filename)).Uri.AbsoluteUri;
        }

        /// <summary>
        /// Save file to amazon.
        /// </summary>
        /// <param name="fileStream">The file bytes.</param>
        /// <param name="filename">The fime name.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file url.</returns>
        private async Task<string> SaveFileToAmazon(byte[] fileStream, string filename, string contentType)
        {
            // Upload image to amazon aws s3 bucket
            using (var client = AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretAccessKey, RegionEndpoint.USEast1))
            {
                TransferUtility utility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                using (var stream = new MemoryStream(fileStream))
                {
                    request.BucketName = AWSBucketName;
                    request.Key = filename;
                    request.InputStream = stream;
                    request.ContentType = contentType;
                    await utility.UploadAsync(request);
                }
                utility.Dispose();
                request.AutoCloseStream = true;
            }

            return AWSUrl + filename;
        }

        /// <summary>
        /// Delete the file from azure.
        /// </summary>
        /// <param name="imagePath">The image path</param>
        /// <returns>None.</returns>
        private async Task DeleteFileFromAzure(string imagePath)
        {
            // Delete image from azure
            var fileName = Path.GetFileName(imagePath);

            var container = GetBlobContainer();

            // Retrieve reference to a blob named "myblob". *** string.Format("{0}/{1}", folderName, fileName)
            var blockBlob = container.GetBlockBlobReference(string.Format("Images/{0}", fileName));

            // Delete the blob if exists.
            var isExists = await blockBlob.ExistsAsync();
            if (isExists)
                await blockBlob.DeleteAsync();
        }

        /// <summary>
        /// Delete the file from amazon.
        /// </summary>
        /// <param name="imagePath">The image path</param>
        /// <returns>None.</returns>
        private async Task DeleteFileFromAmazon(string imagePath)
        {
            // Delete image from amazon aws s3 bucket
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = AWSBucketName,
                Key = imagePath.Split('/').Last()
            };

            using (var client = AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretAccessKey, RegionEndpoint.USEast1))
            {
                var fileInfo = new S3FileInfo(client, AWSBucketName, imagePath.Split('/').Last());
                if (fileInfo.Exists)
                    await client.DeleteObjectAsync(deleteObjectRequest);
            }
        }
    }
}
