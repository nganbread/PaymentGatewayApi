using Checkout.PaymentGateway.Api.Mvc;
using Checkout.PaymentGateway.Service;
using Checkout.PaymentGateway.Service.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Checkout.PaymentGateway.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddPaymentGatewayServices()
                .AddPaymentGatewayApi();

            services
                .Configure<AcquiringBankApiOptions>(_configuration.GetSection("AcquiringBankApi"));

            services
                .AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = "Checkout.com Payment Gateway API - V1",
                        Version = "1"
                    });
                })
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                })
                .AddHttpContextAccessor()
                .AddMvcCore(options =>
                {
                })
                .AddDataAnnotations()
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.DocumentTitle = "Checkout.com Payment Gateway API";
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                })
                .UseMiddleware<DummyAuthenticationMiddleware>()
                .UseMvc();
        }
    }
}
