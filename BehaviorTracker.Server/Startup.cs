using System.Linq;
using System.Net.Mime;
using AutoMapper;
using BehaviorTracker.Repository.Implementations;
using BehaviorTracker.Repository.Interfaces;
using BehaviorTracker.Server.Mappings;
using BehaviorTracker.Service.Implementations;
using BehaviorTracker.Service.Interfaces;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BehaviorTracker.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Repository.BehaviorTrackerDatabaseContext>(options => 
                options.UseSqlite(Repository.BehaviorTrackerDatabaseContext.ConnectionString));
            
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            
           AddAutoMapper(services);

           IocConfiguration.ConfigureIoc(services);

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<Repository.BehaviorTrackerDatabaseContext>();
                #if DEBUG
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                #else
                context.Database.Migrate();
                #endif
                
            }
            
            
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes => { routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}"); });

            app.UseBlazor<BehaviorTracker.Client.Startup>();
            
           
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ServerToServiceProfile>();
                cfg.AddProfile<Service.Mapping.ServiceToRepositoryProfile>();
            });
            
            services.AddAutoMapper();
        }
        
    }
}