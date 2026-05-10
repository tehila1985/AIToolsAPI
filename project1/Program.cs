using Api;
using Api.Middlware;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog.Web;
using Repository;
using Repository.Models;
using Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.Load();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("RedisSettings:ConnectionString").Value;
});

builder.Host.UseNLog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<myDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Tehila"));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStyleService, StyleService>();
builder.Services.AddScoped<IStyleRepository, StyleRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddHttpClient<IAiService, AiService>();
// ===== נוסף עבור הקריאה ל-AI - רישום שירות AI =====
builder.Services.AddHttpClient<IAiService, AiService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseErrorHandling();

app.UseRating();

// ===== נוסף עבור העלאת תמונות למנהל - מאפשר גישה לקבצים סטטיים =====
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();