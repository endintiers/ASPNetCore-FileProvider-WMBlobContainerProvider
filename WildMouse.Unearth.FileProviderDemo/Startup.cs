using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using WildMouse.Extensions.FileProviders.BlobContainer;

namespace WildMouse.Unearth.FileProviderDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // This handles serving files from wwwroot
            app.UseStaticFiles();

            var credentials = new StorageCredentials(Configuration["BlobStore:SasQueryString"]);
            var blobClient = new CloudBlobClient(new Uri(Configuration["BlobStore:Address"]), credentials);
            var documentContainer = blobClient.GetContainerReference("samples");

            // This is for serving from the Documents Folder in the project - which is really a Blob Container
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new BlobContainerFileProvider(documentContainer),
                RequestPath = new PathString("/Documents")
            });

            app.UseMvc();
        }
    }
}
