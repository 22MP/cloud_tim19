using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using SmartApiary.Application;
using SmartApiary.Application.Common.Options;
using SmartApiary.Infrastructure;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SmartApiary.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration)
                .AddApplication();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmartApiary API",
                Version = "v1",
                Description = "API for SmartApiary Web Application"
            });
        });

        return services;
    }

    public static IServiceCollection AddWebApiCors(
        this IServiceCollection services, string reactOrigin)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("_reactAppPolicy", policy =>
            {
                policy.WithOrigins(reactOrigin)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddWebApiSignalR(this IServiceCollection services)
    {
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static IServiceCollection AddWebApiAuth(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Secret))
            };
        });

        services.AddAuthorization();

        return services;
    }
}