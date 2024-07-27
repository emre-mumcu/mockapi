
using MockAPI.AppLib.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Default is JsonNamingPolicy.CamelCase. 
        // Setting it to null will result in property names NOT changing while serializing
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<MockAPI.AppData.AppDbContext>();

builder.Services.AddScoped<IActionLogService, ActionLogService>();

builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await MockAPI.AppData.DataSeeder.SeedData(app.Services);

MockAPI.App.Instance._WebHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
MockAPI.App.Instance._HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

app.Run();

// References
// https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-8.0
// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties?pivots=dotnet-8-0
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-8.0