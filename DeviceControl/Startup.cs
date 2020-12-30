using DeviceControl.Authentication;
using DeviceControl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace DeviceControl
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
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            var apiKeyOptions = Configuration.GetSection("ApiKey").Get<ApiKeyOptions>();

            services.AddAuthentication(ApiKeyExtensions.ApiKeyScheme)
                .AddApiKey(options =>
                {
                    options.KeyName = apiKeyOptions.KeyName;
                    options.KeyValue = apiKeyOptions.KeyValue;
                });

            services.AddAuthorization(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.FallbackPolicy = policy;
            });

            services.AddSingleton<GpioService>();
            services.AddSingleton<HumitureService>();
            services.AddScoped<LedService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Remote Device Control API", Version = "v1" });

                var headerSecurityDefinition = $"Header {ApiKeyExtensions.ApiKeyScheme}";
                var queryStringSecuriryDefinition = $"Query String {ApiKeyExtensions.ApiKeyScheme}";

                options.AddSecurityDefinition(headerSecurityDefinition, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = apiKeyOptions.KeyName,
                    Type = SecuritySchemeType.ApiKey,
                });

                options.AddSecurityDefinition(queryStringSecuriryDefinition, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Query,
                    Name = apiKeyOptions.KeyName,
                    Type = SecuritySchemeType.ApiKey,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = headerSecurityDefinition
                            }
                        }, Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = queryStringSecuriryDefinition
                            }
                        }, Array.Empty<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                app.UseHttpsRedirection();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Remote Device Control API");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
