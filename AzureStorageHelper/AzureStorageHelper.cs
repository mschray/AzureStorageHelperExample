using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AzureStorageHelperLibrary
{
    public static class AzureStorageHelper
    {

        #region ShaSinging experimental

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);

        }

        static string sha256(string password)
        {
            System.Security.Cryptography.HMACSHA256 crypt = new System.Security.Cryptography.HMACSHA256();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            //byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return System.Convert.ToBase64String(crypto);

        }

        //private static string simple(string password)
        //{

        //    System.Security.Cryptography.HMACSHA256 crypt = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["BlobStorageAccountAuthKey"]));
        //    byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
        //    //byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));

        //    return System.Convert.ToBase64String(crypto);

        //} 
        #endregion



        public static async Task<Stream> DownloadBlobAsync(string uri)
        {
#if DEBUG
            Trace.TraceInformation("Downloading image file {0}", uri);
#endif

            // Retrieve reference to a blob. 
            CloudBlockBlob fileBlob = filesBlobContainer.GetBlockBlobReference(uri);

#if DEBUG
            Trace.TraceInformation("Uploaded image file to {0}", fileBlob.Uri.ToString());
#endif

            var httpClient = new HttpClient();
            
            return await httpClient.GetStreamAsync(uri);
            
        }

        private static string GetBlobName(string uri)
        {
            string nameWithoutContainer = string.Empty;

            var theUri = new Uri(uri);

            if (!String.IsNullOrWhiteSpace(theUri.AbsolutePath))
            {

                nameWithoutContainer = theUri.AbsolutePath.Replace(StorageContainerName, "");

                while (nameWithoutContainer.StartsWith("/") || nameWithoutContainer.StartsWith(@"\"))
                {
                    nameWithoutContainer = nameWithoutContainer.Substring(1);
                }
            }

            return nameWithoutContainer;
        }
        public static Uri GenerateSaSToken(string uri,SharedAccessBlobPermissions BlobPermissions, DateTime StartTime, DateTime ExpiracyTime)
        {

            ICloudBlob fileBlob = filesBlobContainer.GetBlobReferenceFromServer(GetBlobName(uri));

            var builder = new UriBuilder(uri);
            builder.Query = fileBlob.GetSharedAccessSignature(
                new SharedAccessBlobPolicy
                {
                    Permissions = BlobPermissions,
                    SharedAccessStartTime = new DateTimeOffset(StartTime),
                    SharedAccessExpiryTime = new DateTimeOffset(ExpiracyTime)
                }).TrimStart('?');

            return builder.Uri;
        }

        public static async Task<Stream> DownloadBlobAsyncSecure(string uri)
        {
#if DEBUG
            Trace.TraceInformation("Downloading image file {0}", uri);
#endif

            string nameWithoutContainer = GetBlobName(uri);

            //http://stackoverflow.com/questions/13456606/azure-access-denied-on-shared-access-signature-for-storage-2-0
            ICloudBlob fileBlob = filesBlobContainer.GetBlobReferenceFromServer(nameWithoutContainer);

#if DEBUG
            Trace.TraceInformation("Download image file to {0}", fileBlob.Uri.ToString());
#endif

            #region Experimental Sas gen code 
            //StringToSign = VERB + "\n" +
            //   Content-Encoding + "\n" +
            //   Content-Language + "\n" +
            //   Content-Length + "\n" +
            //   Content-MD5 + "\n" +
            //   Content-Type + "\n" +
            //   Date + "\n" +
            //   If-Modified-Since + "\n" +
            //   If-Match + "\n" +
            //   If-None-Match + "\n" +
            //   If-Unmodified-Since + "\n" +
            //   Range + "\n" +
            //   CanonicalizedHeaders + 
            //   CanonicalizedResource;

            //string request = @"GET\n\n\n\n\n\n\n\n\n\n\n\n";
            ////string request = @"GET\n";
            //string dt = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss ");
            //dt += "GMT\\n";

            //string cannonicalHeader = string.Format("x-ms-data:{0}\\n{1}\\n", dt, "x-ms-version:2014-02-14");
            ////

            //string cannonResource = "/schraysoft/filestorage/filestorage/mschray@microsoft.com/woman2.png";
            //string sharedKey = request + cannonicalHeader + cannonResource;



            //string access = string.Format("SharedKey schraysoft:{0}", simple(sharedKey));
            ////var Signature = Base64(HMAC-SHA256(UTF8(StringToSign)))
            ////string foo = HttpUtility.HtmlEncode(access);

            //var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://schraysoft.blob.core.windows.net");
            ////httpClient.BaseAddress.Host = "schraysoft.blob.core.windows.net";

            //httpClient.DefaultRequestHeaders.Add("Authorization", access);
            //httpClient.DefaultRequestHeaders.Add("x-ms-date", string.Format("x-ms-data:{0}\\n", dt));
            //httpClient.DefaultRequestHeaders.Add("Host", "schraysoft.blob.core.windows.net");


            ////foo.GetStreamAsync(fileBlob.StorageUri);

            //// generate a full path and file name...
            ////string fname =Path.GetTempFileName();
            ////string path = Path.GetTempPath();

            ////Stream fileStream = File.Create(Path.Combine(new string[] { fname }));

            ////await fileBlob.DownloadToStreamAsync(fileStream);
            //var sasConstraints = new SharedAccessBlobPolicy
            //{
            //    SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
            //    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
            //    Permissions = SharedAccessBlobPermissions.Read
            //};

            //string sasBlobToken = fileBlob.GetSharedAccessSignature(sasConstraints);

            //System.Diagnostics.Trace.WriteLine(fileBlob.Uri + sasBlobToken);
            
            #endregion

            var httpClient = new HttpClient();

            var theUri = GenerateSaSToken(uri, SharedAccessBlobPermissions.Read, DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddMinutes(10));
                                
#if DEBUG
            System.Diagnostics.Trace.WriteLine(theUri.ToString());
#endif
            return await httpClient.GetStreamAsync(theUri);
        }

        
        public static async Task<CloudBlockBlob> UploadStreamToBlobAsync(Stream theFile, string fileName, string customerId, string path)
        {
#if DEBUG
            Trace.TraceInformation("Uploading image file {0}", fileName);
#endif

            // is the path starts with a \ the path combine will drop the customer ID (which would be bad) so remove leading slashes from the path
            if (!String.IsNullOrWhiteSpace(path))
            {
                while (path.StartsWith(@"\"))
                {
                    path = path.Substring(1);
                }
            }
            
            // combine the unique customer id, path I want publish to and the filename
            string blobName = System.IO.Path.Combine(new string[] { customerId, path, Path.GetFileName(fileName) });

            // change \ to /
            blobName = blobName.Replace(@"\", "/");

            // Retrieve reference to a blob. 
        
            CloudBlockBlob fileBlob = filesBlobContainer.GetBlockBlobReference(blobName);


            // Create the blob by uploading a local file.
            using (var fileStream = theFile)
            {
                await fileBlob.UploadFromStreamAsync(fileStream);
            }
#if DEBUG
            Trace.TraceInformation("Uploaded image file to {0}", fileBlob.Uri.ToString());
#endif
            return fileBlob;
        }

        private static CloudBlobContainer filesBlobContainer;
        public static CloudBlobContainer FileBlobContainer
        {
            get
            {
                // Get a reference to the blob container.
                if (filesBlobContainer == null)
                { 
                    filesBlobContainer = Client.GetContainerReference(StorageContainerName);
                }
                  
                return filesBlobContainer;
            }
        }


        public static CloudBlobContainer CreateOrGetContainer(string containerName)
        {
            var container = Client.GetContainerReference(containerName);
            storageContainerName = containerName;
            filesBlobContainer = container;
            return container;
 
        }

        private static string storagePath;
        private static string StoragePath { set { storagePath = value; } }

        private static string storageAccount;
        private static string StorageAccount { set { storageAccount = value; } }

        private static string storageAuthKey;
        private static string StorageAuthKey { set { storageAuthKey = value; } }

        private static string storageContainerName;
        private static string StorageContainerName
        {
            get { return storageContainerName; }
            set { storageContainerName = value; }
        }


        public static void Initialize(string theStoragePath, string theStorageAccountName, string theStorageAuthKey, string theStorageContainerName)
        {
            StoragePath = theStoragePath;
            StorageAccount = theStorageAccountName;
            StorageAuthKey = theStorageAuthKey;
            StorageContainerName = theStorageContainerName;

            // Create the client and set the container setup
            CreateOrGetContainer(StorageContainerName);
        }

        #region CloudBlobClientSetup
        // this code sets up the CloudBlob Storage client for use by the rest of the class

        private static CloudBlobClient client;
        private static CloudBlobClient Client
        {
            get
            {
                if (client == null)
                {
                    // a more secure approach is to change to a Sastoken
                    client = new CloudBlobClient(new Uri(AzureStorageHelper.storagePath),
                        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(AzureStorageHelper.storageAccount, AzureStorageHelper.storageAuthKey));
                }

                return client;
            }
        }
        
        #endregion
    }
}