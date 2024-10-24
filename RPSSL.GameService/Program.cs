using RPSSL.GameService.Hubs;
using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Middlewares;
using RPSSL.GameService.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGameStrategyFactory, GameStrategyFactory>();
builder.Services.AddTransient<IGameSessionService, RedisGameSessionService>();
builder.WebHost.UseUrls("http://+:80");

// Configure CORS to allow any origin, any method, and any header
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowOrigins", policy =>
  {
    policy.AllowAnyOrigin() // Allows all origins
          .AllowAnyMethod() // Allows all HTTP methods (GET, POST, etc.)
          .AllowAnyHeader(); // Allows all headers
  });
});

// Redis configuration
var redisConfig = new ConfigurationOptions
{
  EndPoints = { "rpsl.cache:6379" },
  AbortOnConnectFail = false
};
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Cache"));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));


// Register SignalR services
builder.Services.AddSignalR();
builder.Services.AddHttpClient("ChoiceServiceClient", client =>
{
  client.BaseAddress = new Uri("http://choice-service"); // Use the Docker service name
  client.DefaultRequestHeaders.Add("Accept", "application/json");
  client.Timeout = TimeSpan.FromSeconds(30); // Optional: Set a timeout
});

var app = builder.Build();

app.UseCors("AllowOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapHub<GameHub>("/GameHub");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
