using MadMicro.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var builder = WebApplication.CreateBuilder(args);
builder.AddAppAuthetication();

if(builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
{
    builder.Configuration.AddJsonFile("Ocelot.Production.json", optional: false, reloadOnChange: true);

}
else
{
   builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true); 
}
builder.Services.AddOcelot(builder.Configuration);


var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
