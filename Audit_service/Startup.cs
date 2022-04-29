using Audit_service.DataAccess;
using Audit_service.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Audit_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionService.Set(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Authenticate
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(Jwt =>
            {
                var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
                Jwt.SaveToken = true;
                Jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RequireExpirationTime = false,
                };
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Audit_service", Version = "v2" });
            });
            // Add Entity Frameworks

            var sqlConnectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<KitanoSqlContext>(options => options.UseNpgsql(sqlConnectionString));
            //unitofword
            services.AddScoped<IUnitOfWork>(x => new UnitOfWork(x.GetRequiredService<KitanoSqlContext>()));
            #region redis
            var multiplexer = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection"));
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            var iDb = multiplexer.GetDatabase();
            services.AddSingleton<IDatabase>(iDb);
            services.AddSingleton<IConfiguration>(Configuration);
            //log4net
            services.AddSingleton<ILoggerManager, LoggerManager>();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Audit_service v1"));
            }
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
