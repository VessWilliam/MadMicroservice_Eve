using AutoMapper;
using MadMicro.MessageBus;
using MadMicro.MessageBus.Services.Service;
using MadMicro.Services.OrderAPI;
using MadMicro.Services.OrderAPI.Extensions;
using MadMicro.Services.OrderAPI.DataContext;
using MadMicro.Services.OrderAPI.Services.IService;
using MadMicro.Services.OrderAPI.Services.Service;
using MadMicro.Services.OrderAPI.Utility;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<BackendApiAuthHttpClientHandler>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("Product", u =>
u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthHttpClientHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.AddAppAuthetication();
builder.Services.AddAuthorization();


builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SercetKey").Get<string>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();


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