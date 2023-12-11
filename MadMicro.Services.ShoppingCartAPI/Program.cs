using AutoMapper;
using MadMicro.MessageBus;
using MadMicro.MessageBus.Services.Service;
using MadMicro.Services.ShoppingCartAPI;
using MadMicro.Services.ShoppingCartAPI.Extensions;
using MadMicro.Services.ShoppingCartAPI.DataContext;
using MadMicro.Services.ShoppingCartAPI.Services.IService;
using MadMicro.Services.ShoppingCartAPI.Services.Service;
using MadMicro.Services.ShoppingCartAPI.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<BackendApiAuthHttpClientHandler>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("Coupon", u =>
u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponAPI"]))
    .AddHttpMessageHandler<BackendApiAuthHttpClientHandler>();
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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoppingCart API");
    c.RoutePrefix = string.Empty;
});

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