using Amazon.S3;
using FilesShareApi.FilesCleaner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace FilesShareApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
            
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        { 
            //Database Configuration
            services.Configure<FilesShareApiDbConfig>(Configuration);
            services.AddSingleton<IDbClient, DbClient>();
            services.AddTransient<IFileService, FileService>();

            //AWS S3 services
            services.AddAWSService<IAmazonS3>();
            services.AddSingleton<IAmazonS3, AmazonS3Client>();
            services.AddSingleton<IS3Service, S3Service>();

            //Identity Configuration
            services.AddIdentity<UserEntity, RoleEntity>(config =>
            {
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequiredLength = 8;
            })  
                .AddMongoDbStores<UserEntity, RoleEntity, Guid>
                (
                    Configuration.GetSection("CONNECTION_STRING").Value, "FileShareData"
                );
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Api.Identity.Cookie";
                config.LoginPath = "/login"; 
            }
            );

            services.AddControllers();
            services.AddRouting();

            //Files manager Configuraton
            services.AddSingleton<FilesManager>();
            services.AddSingleton<IHostedService, ScheduledServices>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FilesShareApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FilesShareApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithRedirects("/error?code={404}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
