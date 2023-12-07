using MadMicro.MessageBus;
using MadMicro.MessageBus.Services.Service;
using MadMicro.Services.AuthAPI.DataContext;
using MadMicro.Services.AuthAPI.Models;
using MadMicro.Services.AuthAPI.Service.IService;
using MadMicro.Services.AuthAPI.Services.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("AppSettings:JwtOptions"));
builder.Services.AddScoped<IJwtTokenGenerate, JwtTokenGenerate>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API");
    c.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

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

