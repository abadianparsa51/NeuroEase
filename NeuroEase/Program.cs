using Application.Layer.Interface;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Layer.Data;
using Core.Layer.Helpers;
using Core.Layer.Repository;
using Core.Layer.Services;
using Core.Model.Layer.Entity;
using Core.Model.Layer.Model;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Use Autofac as DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to IServiceCollection
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Add Session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database
builder.Services.AddDbContext<MentalHealthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MentalHealthDbContext>()
    .AddDefaultTokenProviders();

// JWT config setup
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
builder.Services.AddSingleton(jwtConfig);

// JWT Authentication
var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// MediatR registration
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Application.Layer.Authontication.Command.LoginCommand).Assembly,
        typeof(Core.Layer.Handlers.EvaluateRulesCommandHandler).Assembly
    );
});

// Services
builder.Services.AddScoped<IRuleService, RuleService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NeuroEase API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Autofac Container configuration
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register all MediatR handlers
    containerBuilder.RegisterAssemblyTypes(
        typeof(Application.Layer.Authontication.Command.LoginCommand).Assembly,
        typeof(Core.Layer.Handlers.EvaluateRulesCommandHandler).Assembly
    )
    .AsClosedTypesOf(typeof(IRequestHandler<,>))
    .InstancePerLifetimeScope();

    // Repositories
    containerBuilder.RegisterType<AuthRepository>().As<IAuthenticationRepository>().InstancePerLifetimeScope();

    // Helpers
    containerBuilder.RegisterType<JwtHelper>().As<IJwtHelper>().InstancePerLifetimeScope();

    // Services
    containerBuilder.RegisterType<RuleService>().As<IRuleService>().InstancePerLifetimeScope();
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();