using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WildMouse.Extensions.FileProviders.BlobContainer
{
    public class BlobContainerDirectoryContents : IDirectoryContents
    {
        CloudBlobContainer _container;
        int _maxBlobs;

        public bool Exists { get; set; }
        public int MaxBlobs { get { return _maxBlobs; } }

        // By default will only return the first 100 blobs
        public BlobContainerDirectoryContents(CloudBlobContainer container, int maxBlobs = 100)
        {
            _container = container;
            if (maxBlobs > 5000)
            {
                throw new ApplicationException("blob paging not yet implemented - max 5000 blobs");
            }
            _maxBlobs = maxBlobs;
            Exists = container.ExistsAsync().GetAwaiter().GetResult();
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            var blobs = _container.ListBlobsSegmentedAsync(string.Empty, false, BlobListingDetails.Metadata, _maxBlobs, null, null, null).Result.Results;

            // Note - this implementation generates a lot of REST calls (1 per blob)
            foreach (var blob in blobs)
            {
                yield return new BlobContainerFileInfo(blob);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
