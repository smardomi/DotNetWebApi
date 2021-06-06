using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetWebApi.Common;
using DotNetWebApi.Common.Exceptions;
using DotNetWebApi.DataLayer.EFContext;
using DotNetWebApi.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetWebApi.IocConfig.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseOracle(configuration.GetConnectionString("Oracle"));
                //Tips
                //Automatic client evaluation is no longer supported. This event is no longer generated.
                //This line is no longer needed.
                //.ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));
            });
        }

        public static void AddMinimalMvc(this IServiceCollection services)
        {
            //https://github.com/aspnet/AspNetCore/blob/0303c9e90b5b48b309a78c2ec9911db1812e6bf3/src/Mvc/Mvc/src/MvcServiceCollectionExtensions.cs
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter()); //Apply AuthorizeFilter as global filter to all actions

                //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
                //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
                //Use this filter when use cookie 
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                //options.UseYeKeModelBinder();

                //options.Filters.Add(
                //    new ProducesAttribute("application/json"));
                //options.Filters.Add(
                //    new ConsumesAttribute("application/json"));

                //options.Filters.Add(
                //    new SwaggerResponseAttribute(StatusCodes.Status400BadRequest));
                //options.Filters.Add(
                //    new SwaggerResponseAttribute(StatusCodes.Status401Unauthorized));
                //options.Filters.Add(
                //    new SwaggerResponseAttribute(StatusCodes.Status200OK));
                //options.Filters.Add(
                //    new SwaggerResponseAttribute(StatusCodes.Status500InternalServerError));

                //options.Filters.Add(
                //    new SwaggerResponseExampleAttribute(StatusCodes.Status400BadRequest,typeof(ApiResult)));

                options.ReturnHttpNotAcceptable = true;

            })
            .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                })
            .AddViewLocalization()
            .AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.Converters.Add(new StringEnumConverter());
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //option.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(option =>
            {
                var supportedCultures = new[] {
                    new CultureInfo("fa-IR")
                };
                option.DefaultRequestCulture = new RequestCulture(culture: "fa-IR",uiCulture: "fa-IR");
                option.SupportedCultures = supportedCultures;
                option.SupportedUICultures = supportedCultures;
                option.RequestCultureProviders.Insert(1, new RouteDataRequestCultureProvider());
            });
        }

        public static void AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins(
                            "https://smardomi.ir") //Note:  The URL must be specified without a trailing slash (/).
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.SetIsOriginAllowed((host) => true)
                .AllowCredentials());
            });
        }

        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddOptions<JwtSettings>()
            //    .Bind(configuration.GetSection("SiteSettings:JwtSettings"))
            //    .Validate(bearerTokens => bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationDays, "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");
            services.Configure<SiteSettings>(configuration.GetSection(nameof(SiteSettings)));
        }

        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            // Only needed for custom roles.
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
            //    options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
            //    options.AddPolicy(CustomRoles.Editor, policy => policy.RequireRole(CustomRoles.Editor));
            //});

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                        await tokenValidatorService.ValidateAsync(context, context.HttpContext.RequestAborted);
                    },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                    }
                };
            });
        }

        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                //url segment => {version}
                options.AssumeDefaultVersionWhenUnspecified = true; //default => false;
                options.DefaultApiVersion = new ApiVersion(1, 0); //v1.0 == v1
                options.ReportApiVersions = true;

                //ApiVersion.TryParse("1.0", out var version10);
                //ApiVersion.TryParse("1", out var version1);
                //var a = version10 == version1;

                //options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
                // api/posts?api-version=1

                //options.ApiVersionReader = new UrlSegmentApiVersionReader();
                // api/v1/posts

                //options.ApiVersionReader = new HeaderApiVersionReader(new[] { "Api-Version" });
                // header => Api-Version : 1

                //options.ApiVersionReader = new MediaTypeApiVersionReader()

                //options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"), new UrlSegmentApiVersionReader())
                // combine of [querystring] & [urlsegment]
            });
        }
    }
}
