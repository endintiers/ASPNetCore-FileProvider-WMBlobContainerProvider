using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WildMouse.Extensions.FileProviders.BlobContainer
{
    public class BlobContainerFileInfo : IFileInfo
    {
        CloudBlobContainer _container;
        CloudBlockBlob _blob;
        String _fileName;

        public bool Exists { get; set; }
        public long Length { get; set; }
        public string PhysicalPath { get; set; }
        public string Name { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public bool IsDirectory { get; set; }

        public BlobContainerFileInfo(CloudBlobContainer container, string fileName)
        {
            _container = container;
            _fileName = fileName.Substring(1); // Ignore slash at the beginning
            GetBlobInfo();
        }

        public BlobContainerFileInfo(IListBlobItem blob)
        {
            _container = blob.Container;
            _fileName = ((CloudBlob) blob).Name; //Naughty :-)
            GetBlobInfo();
        }

        private void GetBlobInfo()
        {
            _blob = _container.GetBlockBlobReference(_fileName);
            Exists = _blob.ExistsAsync().GetAwaiter().GetResult();
            if (Exists)
            {
                _blob.FetchAttributesAsync().GetAwaiter().GetResult();
                Length = _blob.Properties.Length;
                PhysicalPath = _blob.StorageUri.PrimaryUri.ToString();
                Name = _blob.Name;
                LastModified = _blob.Properties.LastModified ?? DateTimeOffset.Now;
                IsDirectory = false;
            }
        }

        public Stream CreateReadStream()
        {
            if (Exists)
            {
                var blob = _container.GetBlockBlobReference(_fileName);
                try
                {
                    var ms = new MemoryStream();
                    blob.DownloadToStreamAsync(ms).GetAwaiter().GetResult();
                    ms.Position = 0;
                    return ms;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
                return null;
        }

    }
}
