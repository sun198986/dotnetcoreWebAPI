using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Routine.Api.Data;
using Routine.Api.Services;

namespace Routine.Api
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
            //����
            services.AddHttpCacheHeaders(expires =>
                {
                    expires.MaxAge = 60;
                    expires.CacheLocation = CacheLocation.Private;
                },
                validation =>
                {
                    validation.MustRevalidate = true;
                });

        services.AddResponseCaching();
            services.AddControllers(setup =>
                    {
                        setup.ReturnHttpNotAcceptable = true; //����accept
                        setup.CacheProfiles.Add("120sCacheProfile",new CacheProfile
                        {
                            Duration = 120
                        });
                        //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                        //setup.OutputFormatters.Insert(0,new XmlDataContractSerializerOutputFormatter());//˳�����Ĭ�����
                    }
                )
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setup =>//������Ϣ������� FluentValidation
                {
                    setup.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "www.baidu.com",
                            Title = "�д���",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "�뿴��ϸ��Ϣ",
                            Instance = context.HttpContext.Request.Path
                        };

                        problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.Configure<MvcOptions>(config =>
            {
                var newtonSoftJsonOutputFormatter =
                    config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddDbContext<RoutineDbContext>(option =>
                {
                    option.UseSqlite("Data Source=routine.db");
                }
            );

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyChckerService, PropertyChckerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error");
                    });
                });
            }

            //app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
