using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AuctionDbContext>(opt => {
  opt.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionStrings"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
  DbInitializer.InitDb(app);
}
catch (Exception e)
{
  
  Console.WriteLine(e.Message);
}

app.Run();
