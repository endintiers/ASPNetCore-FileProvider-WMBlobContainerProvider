using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace WildMouse.Extensions.FileProviders.BlobContainer
{
    public class BlobContainerFileProvider : IFileProvider
    {
        private CloudBlobContainer _container;
        private BlobContainerDirectoryContents _contents;

        public BlobContainerFileProvider(CloudBlobContainer container)
        {
            _container = container;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (_contents == null) _contents = new BlobContainerDirectoryContents(_container);
            return _contents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return new BlobContainerFileInfo(_container, subpath);
        }

        public Microsoft.Extensions.Primitives.IChangeToken Watch(string filter)
        {
            // For simplicity consider the blob container as static
            throw new NotImplementedException();
        }

    }
}
