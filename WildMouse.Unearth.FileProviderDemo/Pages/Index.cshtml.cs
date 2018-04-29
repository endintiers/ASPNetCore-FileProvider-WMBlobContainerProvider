using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using WildMouse.Extensions.FileProviders.BlobContainer;

namespace WildMouse.Unearth.FileProviderDemo.Pages
{
    public class IndexModel : PageModel
    {
        private IConfiguration _configuration;

        public IFileInfo[] Files { get; set; }

        // Inject some config goodness...
        public IndexModel(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public void OnGet()
        {
            var credentials = new StorageCredentials(_configuration["BlobStore:SasQueryString"]);
            var blobClient = new CloudBlobClient(new Uri(_configuration["BlobStore:Address"]), credentials);
            var documentContainer = blobClient.GetContainerReference("samples");

            var docFolder = new BlobContainerDirectoryContents(documentContainer);
            Files = docFolder.ToArray();
        }
    }
}
