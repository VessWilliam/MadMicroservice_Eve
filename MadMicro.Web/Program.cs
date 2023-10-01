using MadMicro.Web.Services.IService;
using MadMicro.Web.Services.Service;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();


StaticDetail.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
StaticDetail.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
StaticDetail.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
StaticDetail.ShopCartAPIBase = builder.Configuration["ServiceUrls:ShopCartAPI"];

builder.Services.AddHttpClient<ICouponService, CouponServices>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IShopCartService, ShopCartService>();

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponServices>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IProductService,ProductService>();   
builder.Services.AddScoped<ITokenProvider, TokenProvider>();    
builder.Services.AddScoped<IShopCartService, ShopCartService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(10);
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDeniel";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
