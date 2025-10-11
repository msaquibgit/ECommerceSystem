using Messaging.Common.Extension;
using Messaging.Common.Options;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interface;
using ProductService.Application.Mappings;
using ProductService.Application.Messaging;
using ProductService.Application.Services;
using ProductService.Contract.Messaging;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Messaging;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add DbContext

builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Register services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
var mq = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>()!;

builder.Services.AddRabitMQ(mq.HostName, mq.UserName, mq.Password, mq.VirtualHost);
builder.Services.AddScoped<IOrderPlacedHandler, OrderPlacedHandler>();
builder.Services.AddHostedService<OrderPlacedConsumer>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
