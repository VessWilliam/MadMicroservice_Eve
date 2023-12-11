using MadMicro.Services.EmailAPI.DataContext;
using MadMicro.Services.EmailAPI.Extensions;
using MadMicro.Services.EmailAPI.Messaging;
using MadMicro.Services.EmailAPI.Services.IService;
using MadMicro.Services.EmailAPI.Services.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionBuilder.Options));

//RabbitMQ
builder.Services.AddHostedService<RabbitMQAuthConsumer>();
builder.Services.AddHostedService<RabbitMQShoppingCartConsumer>();

//Azure Bus
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email API");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.UserAzureServiceBusConsumer();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
            _db.Database.Migrate();
    }
}