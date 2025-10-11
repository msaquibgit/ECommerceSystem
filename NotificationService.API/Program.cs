using Messaging.Common.Extension;
using Messaging.Common.Options;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Handlers;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Messaging;
using NotificationService.Application.Services;
using NotificationService.Application.Utilities;
using NotificationService.Contract.Interfaces;
using NotificationService.Contract.Messaging;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.BackgroundJobs;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationService, NotificationService.Application.Services.NotificationService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISMSService, SMSService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>();

// Channel Handlers
builder.Services.AddScoped<INotificationChannelHandler, EmailChannelHandler>();
builder.Services.AddScoped<INotificationChannelHandler, SmsChannelHandler>();
builder.Services.AddScoped<INotificationChannelHandler, InAppChannelHandler>();

//Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();


// Bind RabbitMQ config
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
var mq = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>()!;

// Register RabbitMQ channel (you likely already have AddRabbitMq in Messaging.Common)
builder.Services.AddRabitMQ(mq.HostName, mq.UserName, mq.Password, mq.VirtualHost);

// Register handler
builder.Services.AddScoped<IOrderPlacedHandler, OrderPlacedHandler>();

// Register consumer
builder.Services.AddHostedService<OrderPlacedConsumer>();

builder.Services.AddScoped<INotificationProcessor, NotificationService.Application.Services.NotificationService>();

// Register Background Worker
builder.Services.AddHostedService<NotificationWorker>();




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
