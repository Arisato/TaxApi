using DataEF;
using Microsoft.EntityFrameworkCore;
using TaxLedgerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

//Add Core Services
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen().AddSwaggerGenNewtonsoftSupport();
builder.Services.AddOpenApi();

//Add DB Context
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connection));

//Add Local Services
builder.Services.AddLocalServices();

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
