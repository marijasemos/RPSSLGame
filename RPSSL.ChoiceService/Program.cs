using RPSSL.ChoiceService.Interfaces;
using RPSSL.ChoiceService.Middlewares;
using RPSSL.ChoiceService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string randomNumberApiBaseUrl = builder.Configuration.GetSection("ExternalServices:RandomNumberApiBaseUrl").Value;

if (string.IsNullOrWhiteSpace(randomNumberApiBaseUrl))
{
  throw new InvalidOperationException("The RandomNumberApiBaseUrl configuration is missing or empty.");
}

// Register HttpClient with the base address from configuration
builder.Services.AddHttpClient<IRandomNumberService, RandomNumberService>(client =>
{
  client.BaseAddress = new Uri(randomNumberApiBaseUrl);
  client.Timeout = TimeSpan.FromSeconds(30);
  client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.WebHost.UseUrls("http://+:80");
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins", builder =>
  {
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
  });
});
var app = builder.Build();
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
