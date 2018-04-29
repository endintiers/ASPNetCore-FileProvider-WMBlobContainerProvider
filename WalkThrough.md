
This is a walkthrough of the (important) code in [ASPNetCore-FileProvider-WMBlobContainerProvider](https://github.com/endintiers/ASPNetCore-FileProvider-WMBlobContainerProvider#aspnetcore-fileprovider-wmblobcontainerprovider). A running example can be found at: https://customfileproviders.azurewebsites.net.

Open the solution. Note there is a Documents folder under the FileProviderDemo project. It is just a placeholder.

In FileProviderDemo, Startup.cs, Configure method:
   
    var credentials = new StorageCredentials(Configuration["BlobStore:SasQueryString"]);
    var blobClient = new CloudBlobClient(new Uri(Configuration["BlobStore:Address"]), credentials);
    var documentContainer = blobClient.GetContainerReference("samples");
    
    // This is for serving from the Documents Folder in the project - which is really a Blob Container
    app.UseStaticFiles(new StaticFileOptions()
    {
	    FileProvider = new BlobContainerFileProvider(documentContainer),
	    RequestPath = new PathString("/Documents")
    });
This gets some (readonly) credentials from appsettings.json, then uses them to reference a container in a blob store.
The default UseStaticFiles above this allows serving of files from wwwroot (like pretty images). **This UseStaticFiles has options**. The **PathString** says it is going to take over serving files from the Documents folder. A custom **FileProvider** is instantiated with the container passed to it's constructor.
*That's all we have to do to serve files in that container as it they were in the Documents folder*.

When a file is requested from the Documents folder, the .NET Core FileSystem calls the GetFileInfo method of the BlobContainerFileProvider that is attached to that folder which returns an BlobContainerFileInfo.

    public IFileInfo GetFileInfo(string subpath)
    {
    	return new BlobContainerFileInfo(_container, subpath);
    }

The BlobContainerFileInfo has a GetStream method that the .NET Core FileSystem can (and does) use to fetch the file's contents from the blob store.

In Index.cshtml we exercise the other class in the provider, BlobContainerDirectoryContents, which knows how to return an Enumerator over the files in the container.

    var credentials = new StorageCredentials(_configuration["BlobStore:SasQueryString"]);
    var blobClient = new CloudBlobClient(new Uri(_configuration["BlobStore:Address"]), credentials);
    var documentContainer = blobClient.GetContainerReference("samples");
    
    var docFolder = new BlobContainerDirectoryContents(documentContainer);
    Files = docFolder.ToArray();

This list is then displayed on the page with links that try to open the documents in another tab. Works best with Edge (of course).

And that's pretty much it. This implementation has several problems (most of which are noted in code) and is far too synchronous, but that is the price for keeping the LOC down to a dull roar.
