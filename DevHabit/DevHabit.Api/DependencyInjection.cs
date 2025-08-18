using Asp.Versioning;
using DevHabit.Api.AutoMapper;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Middleware;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.IdentityModel.Tokens;
using DevHabit.Api.Settings;
using System.Text;
namespace DevHabit.Api;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(option =>
        {
            option.ReturnHttpNotAcceptable = true;
        }).AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
          .AddXmlSerializerFormatters();

        builder.Services.Configure<MvcOptions>(options =>
        {
            NewtonsoftJsonOutputFormatter formatter = options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().First();
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV2);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJson);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV2);
        });

        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions =  true;
                options.ApiVersionSelector = new DefaultApiVersionSelector(options);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new MediaTypeApiVersionReader(),
                    new MediaTypeApiVersionReaderBuilder()
                    .Template("application/vnd.dev-habit.hateoas.{version}+json")
                    .Build());
            })
            .AddMvc();

        builder.Services.AddOpenApi();

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandlers(this WebApplicationBuilder builder) {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder) {

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

        builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
            })
            .AddSqlClientInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
            })
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:18888");
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:18888");
            });
    });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:18888");
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder) {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfiles>());

        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<HabitDto, Habit>>(_ => HabitMappings.SortMapping);

        builder.Services.AddTransient<DataShapingService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<LinkService>();
        builder.Services.AddTransient<TokenProvider>();

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<UserContext>();

        builder.Services.AddScoped<GitHubAccessTokenService>();
        builder.Services.AddTransient<GitHubService>();
        builder.Services.AddHttpClient("github")
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.github.com");

                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("DevHabit", "1.0"));

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            });

        builder.Services.Configure<EncryptionOptions>(builder.Configuration.GetSection("Encryption"));
        builder.Services.AddTransient<EncrytionService>();
        return builder;
    }

    public static WebApplicationBuilder AddAthenticationServices(this WebApplicationBuilder builder) {
        builder.Services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

        builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("Jwt"));

        JwtAuthOptions jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
            .AddJwtBearer(options =>
            {
                if(jwtAuthOptions is not null)
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateLifetime = true,
                        ValidIssuer = jwtAuthOptions.Issuer,
                        ValidAudience = jwtAuthOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key))


                    };
                }
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
