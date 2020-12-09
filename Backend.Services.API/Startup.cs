
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using MyShopOnLine.Backend.Data;
using MyShopOnLine.Backend.Services;

namespace MyShopOnLine.Backend.API
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
            services.AddDbContext<MyShopOnLineDataContext>(
                x => x.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"))
                        .LogTo(System.Console.WriteLine, new[] {
                            Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandCreating,
                            Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted},
                            Microsoft.Extensions.Logging.LogLevel.Information));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Shop OnLine Backend Services API", Version = "v1" });
            });

            // Injection of dependencies.
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MyShopOnLineDataContext dbContext)
        {
            dbContext.Database.Migrate();

            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Services API v1"));
            //}

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
