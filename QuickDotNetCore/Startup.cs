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
            //添加服务调用
            //services.AddScoped<IGear_RowService, Gear_RowService>();

            //数据库注入
            services.AddSqlsugarSetup(Configuration);

            //model实体映射到数据库
            ModelsAutoMapper.Mapper(Configuration);

            //Service层注入
            RegisterServices.Register(services);

            //DAO层注入
            RegisterDAOs.Register(services);



            #region 一、接口限流  
            services.AddOptions();
            //需要存储速率限制计算器和ip规则
            services.AddMemoryCache();

            //从appsettings.json中加载常规配置，IpRateLimiting与配置文件中节点对应
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //从appsettings.json中加载Ip规则
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddInMemoryRateLimiting();
            //注入计数器和规则存储
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
                    Description = "请输入Token令牌，前置Bearer 示例：Bearer ****",
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
                    //此处为权限验证失败后触发的事件
                    OnChallenge = context =>
                    {
                        //此处代码为终止.Net Core默认的返回类型和数据结果
                        context.HandleResponse();
                        Console.WriteLine(context.Error);
                        var payload = JsonConvert.SerializeObject(BaseResponse<string>.Fail("权限验证失败,请重新登录"));
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context => {
                        Console.WriteLine("OnAuthenticationFailed");
                        var payload = JsonConvert.SerializeObject(BaseResponse<string>.Fail("权限验证失败,请重新登录 OnAuthenticationFailed"));
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(-1);
                    },
                    OnForbidden = context => {
                        Console.WriteLine("OnForbidden");
                        context.Fail(new Exception("OnForbidden"));
                        var payload = JsonConvert.SerializeObject(BaseResponse<string>.Fail("权限验证失败,请重新登录 OnForbidden"));
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
            #region 二、接口限流  
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //配置（解析器、计数器密钥生成器）
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

            //#region 三、接口限流  
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
        /// 数据库驱动注入
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureMySqlService(IServiceCollection services) {
            // 10秒入门
            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                //ConfigId="db01"  多租户用到
                ConnectionString = BaseDBConfig.ConnectionString,
                DbType = IocDbType.MySql,
                IsAutoCloseConnection = true//自动释放
            }); //多个库就传List<IocConfig>


            //配置参数
            SugarIocServices.ConfigurationSugar(db =>
            {
                db.Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(sql);
                };
                //设置更多连接参数
                //db.CurrentConnectionConfig.XXXX=XXXX
                //db.CurrentConnectionConfig.MoreSettings=new ConnMoreSettings(){}
                //二级缓存设置
                //db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
                //{
                // DataInfoCacheService = myCache //配置我们创建的缓存类
                //}
                //读写分离设置
                //laveConnectionConfigs = new List<SlaveConnectionConfig>(){...}

                /*多租户注意*/
                //单库是db.CurrentConnectionConfig 
                //多租户需要db.GetConnection(configId).CurrentConnectionConfig 
            });
        }
    }
}
