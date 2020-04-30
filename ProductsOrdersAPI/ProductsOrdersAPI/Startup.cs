using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductsOrders.BLL;
using ProductsOrders.DAL.Models.Mongo;
using ProductsOrders.DAL.Repository;
using ProductsOrders.DAL.Repository.Interfaces;

namespace ProductsOrdersAPI
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
            services.AddControllers();
            services.AddMemoryCache();
            services.Configure<Mongosettings>(options => Configuration.GetSection("Mongosettings").Bind(options));

            services.AddScoped<IMongoDBContext, MongoDBContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IInvoiceRuleService, InvoiceRuleService>();
            services.AddScoped<IInvoiceRuleRepository, InvoiceRuleRepository>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title = "Produtcs Orders API",
                    Version = "v1.0",
                    Description = "v1.0 API"
                });

                // options.IncludeXmlComments(GetIncludeXmlComments());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/ProductsOrdersAPI-{Date}.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    url: "/swagger/v1.0/swagger.json",
                    name: "Product Orders API V1.0"); ;
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}