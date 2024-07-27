namespace MockAPI;

public sealed class App
{
    // https://csharpindepth.com/articles/singleton (Sixth version)

    private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());

    public static App Instance { get { return appInstance.Value; } }

    private App()
    {
        _DataConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("data.json", true)
            .Build();
    }

    public IConfiguration _DataConfiguration { get; set; }
    public IHttpContextAccessor _HttpContextAccessor { get; set; } = null!;
    public IWebHostEnvironment _WebHostEnvironment { get; set; } = null!;
}