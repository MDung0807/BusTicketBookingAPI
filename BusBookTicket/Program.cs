﻿using AutoMapper;
using BusBookTicket.Auth.Repositories.AuthRepository;
using BusBookTicket.Auth.Repositories.RoleRepository;
using BusBookTicket.Auth.Security;
using BusBookTicket.Auth.Services.AuthService;
using BusBookTicket.Auth.Services.RoleService;
using BusBookTicket.Core.Models.EntityFW;
using BusBookTicket.Configs;
using BusBookTicket.CustomerManage.Repositories;
using BusBookTicket.CustomerManage.Services;
using BusBookTicket.Exceptions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using BusBookTicket.BusStationManage.Repositories;
using BusBookTicket.BusStationManage.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;

        #region -- Config auto mapping --
        var mapperConfigs = new MapperConfiguration(cfg =>
        cfg.AddProfile(new MappingProfile())
      );
        IMapper mapper = mapperConfigs.CreateMapper();
        services.AddSingleton(mapper);
        #endregion -- Config auto mapping --

        #region -- Authen --

        SHA256 sha256 = SHA256.Create();
        var secretBytes = Encoding.UTF8.GetBytes("BachelorOfEngineeringThesisByMinhDung");
        var symmetricKey = sha256.ComputeHash(secretBytes);

        // Add services to the container.
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Các cấu hình kiểm tra token
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                    ClockSkew = TimeSpan.Zero
                };

                options.RequireHttpsMetadata = false; // Set to true if you require HTTPS
                options.SaveToken = true;
            });

        #endregion -- Authen --
        
        services.AddAuthorization();
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddDbContext<AppDBContext>(
            options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDB")));
        services.AddSwaggerGen();
        
        
        #region -- Scoped --
        ScopedConfigs.Configure(services: services);

        #endregion -- Scoped --

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseRouting();
        app.UseCors(options =>
        {
            options.AllowAnyOrigin();
            options.AllowAnyMethod();
            options.AllowAnyHeader();
        });
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseEndpoints(endpoints =>
        endpoints.MapControllers()) ;
        
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        app.Run();
    }
}