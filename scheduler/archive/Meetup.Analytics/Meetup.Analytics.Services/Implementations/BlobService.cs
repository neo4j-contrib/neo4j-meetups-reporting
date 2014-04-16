using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Meetup.Analytics.Services.Implementations
{
    /// <summary>
    /// Utilities class used to make blob service requests.
    /// </summary>
    public static class BlobService
    {
        public static Uri PutBlobGetUri(string blobFolder, string blobName, Stream blobStream, string query)
        {
            var storage = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudBlobClient blobClient = storage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.ListContainers(blobFolder, ContainerListingDetails.All).FirstOrDefault();
            if (blobContainer == null)
            {
                blobContainer = blobClient.GetContainerReference(blobFolder);
                blobContainer.Create();
            }
            CloudBlobDirectory blobDirectory = blobContainer.GetDirectoryReference(HttpUtility.UrlDecode(query));
            CloudBlockBlob cloudBlob = blobDirectory.GetBlockBlobReference(blobName);

            byte[] content = new byte[blobStream.Length];
            blobStream.Read(content, 0, (int)blobStream.Length);
            var blockLength = 400 * 1024;
            var numberOfBlocks = ((int)content.Length / blockLength) + 1;
            string[] blockIds = new string[numberOfBlocks];

            Parallel.For(0, numberOfBlocks, x =>
            {
                var blockId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                var currentLength = Math.Min(blockLength, content.Length - (x * blockLength));

                using (var memStream = new MemoryStream(content, x * blockLength, currentLength))
                {
                    cloudBlob.PutBlock(blockId, memStream, null);
                }
                blockIds[x] = blockId;
            });

            cloudBlob.PutBlockList(blockIds);

            // Set properties
            cloudBlob.Properties.ContentType = "text/csv";
            cloudBlob.SetProperties();

            // Quickly clear this data from memory
            blobStream.Dispose();

            return cloudBlob.Uri;
        }
    }
}
