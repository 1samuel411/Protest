using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Drawing;
using System.Threading;
using System.Configuration;
using ProtestBackend.DLL;
using Microsoft.Win32;

namespace ProtestBackend.DAL
{
    public class StorageManager
    {
        private const string STORAGENAMEFIELD = "ProtestStorage"; // Stoage Connection string name from Web.config
        private const string STORAGEURL = "https://protest.blob.core.windows.net";

        public static string UploadToStorage(HttpPostedFileBase file, string containerName, int id = 0, string name = null)
        {
            // Generate a name for the blob
            if(name == null)
            {
                name = id + "_" + Guid.NewGuid().ToString() + "_" + Parser.UnparseDate(DateTime.Now).Replace('.', '_') + GetDefaultExtension(file.ContentType);
            }

            CloudStorageAccount storageAcc = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[STORAGENAMEFIELD]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAcc.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Retrieve reference to a blob.
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

            blockBlob.UploadFromStream(file.InputStream);

            return STORAGEURL + "/" + containerName + "/" + name;
        }

        public static string GetDefaultExtension(string mimeType)
        {
            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }
    }
}