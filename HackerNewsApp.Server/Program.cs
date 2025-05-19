using HackerNewsApp.Server.Clients;
using HackerNewsApp.Server.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();

builder.Services.AddRefitClient<IHackerNewsClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/"));


var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });

    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")); 
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
