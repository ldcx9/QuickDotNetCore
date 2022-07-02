using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Config;
using QuickDotNetCore.Src.vo;
using WeChat.Api.Extensions;
using SqlSugar.IOC;
using TingBao_API.Src.utils;

namespace QuickDotNetCore
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
            //��ӷ������
            //services.AddScoped<IGear_RowService, Gear_RowService>();

            //���ݿ�ע��
            services.AddSqlsugarSetup(Configuration);

            //modelʵ��ӳ�䵽���ݿ�
            ModelsAutoMapper.Mapper(Configuration);

            //Service��ע��
            RegisterServices.Register(services);

            //DAO��ע��
            RegisterDAOs.Register(services);



            #region һ���ӿ�����  
            services.AddOptions();
            //��Ҫ�洢�������Ƽ�������ip����
            services.AddMemoryCache();

            //��appsettings.json�м��س������ã�IpRateLimiting�������ļ��нڵ��Ӧ
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //��appsettings.json�м���Ip����
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddInMemoryRateLimiting();
            //ע��������͹���洢
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            #endregion

            services.AddCors(delegate (CorsOptions options)
            {
                options.AddPolicy("CorsPolicy", delegate (CorsPolicyBuilder builder)
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(delegate (SwaggerGenOptions c)
            {
                c.SwaggerDoc(Configuration["Swagger:Version"], new OpenApiInfo
                {
                    Version = Configuration["Swagger:Version"],
                    Title = Configuration["Swagger:Title"],
                    Description = Configuration["Swagger:Description"]
                });
                c.IncludeXmlComments(Path.Combine(Path.GetDirectoryName(typeof(Startup).Assembly.Location), Assembly.GetExecutingAssembly().GetName().Name + ".xml"), includeControllerXmlComments: true);
                c.CustomSchemaIds((Type type) => type.FullName);
             //   c.DocumentFilter<ApiGroupSortFilter>(Array.Empty<object>());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "������Token���ƣ�ǰ��Bearer ʾ����Bearer ****",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                } });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = Const.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = Const.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey))
                };
                options.Events = new JwtBearerEvents()
                {
                    //�˴�ΪȨ����֤ʧ�ܺ󴥷����¼�
                    OnChallenge = context =>
                    {
                        //�˴�����Ϊ��ֹ.Net CoreĬ�ϵķ������ͺ����ݽ��
                        context.HandleResponse();
                        Console.WriteLine(context.Error);
                        var payload = JsonConvert.SerializeObject(BaseResponse<string>.Fail("Ȩ����֤ʧ��,�����µ�¼"));
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context => {
                        Console.WriteLine("OnAuthenticationFailed");
                        var payload = JsonConvert.SerializeObject(BaseResponse<string>.Fail("Ȩ����֤ʧ��,�����µ�¼ OnAuthenticationFailed"));
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(-1);
                    },
                    OnForbidden = context => {
                        Console.WriteLine("OnForbidden");
                        context.Fail(new Exception("OnForbidden"));
                        var payload = JsonConvert.SerializeObject(BaseResponse<string>.Fail("Ȩ����֤ʧ��,�����µ�¼ OnForbidden"));
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(-2);
                    }
                };
            });



            services.AddControllers(delegate (MvcOptions p)
            {
                p.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddJsonOptions(delegate (JsonOptions o)
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            }).AddNewtonsoftJson(delegate (MvcNewtonsoftJsonOptions o)
            {
                o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                o.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
            });
            #region �����ӿ�����  
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //���ã�����������������Կ��������
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //ServiceLocator.SetServices(app.ApplicationServices);

            //#region �����ӿ�����  
            //app.UseIpRateLimiting();
            //#endregion
            app.UseHttpsRedirection();
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseSwagger().UseSwaggerUI(delegate (SwaggerUIOptions c)
            {
                c.SwaggerEndpoint("/swagger/" + Configuration["Swagger:Version"] + "/swagger.json", "QuickDotNetCore " + Configuration["Swagger:Version"]);
                c.RoutePrefix = Configuration["Swagger:RoutePrefix"];
                c.DocExpansion(DocExpansion.None);
                c.DefaultModelExpandDepth(1);
            });
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(delegate (IEndpointRouteBuilder endpoints)
            {
                endpoints.MapControllers();
            });
        }


        /// <summary>
        /// ���ݿ�����ע��
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureMySqlService(IServiceCollection services) {
            // 10������
            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                //ConfigId="db01"  ���⻧�õ�
                ConnectionString = BaseDBConfig.ConnectionString,
                DbType = IocDbType.MySql,
                IsAutoCloseConnection = true//�Զ��ͷ�
            }); //�����ʹ�List<IocConfig>


            //���ò���
            SugarIocServices.ConfigurationSugar(db =>
            {
                db.Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(sql);
                };
                //���ø������Ӳ���
                //db.CurrentConnectionConfig.XXXX=XXXX
                //db.CurrentConnectionConfig.MoreSettings=new ConnMoreSettings(){}
                //������������
                //db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
                //{
                // DataInfoCacheService = myCache //�������Ǵ����Ļ�����
                //}
                //��д��������
                //laveConnectionConfigs = new List<SlaveConnectionConfig>(){...}

                /*���⻧ע��*/
                //������db.CurrentConnectionConfig 
                //���⻧��Ҫdb.GetConnection(configId).CurrentConnectionConfig 
            });
        }
    }
}
