```cs
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MockAPI.AppLib.Filters;

namespace MockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private static IEnumerable<Type> FindTypesImplementingInterface<T>(IEnumerable<Assembly> assemblies)
        {
            var interfaceType = typeof(T);

            return assemblies.SelectMany(a => a.GetTypes())
                             .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }

        [HttpGet("[action]")]
        [ActionLogFilter]
        public IActionResult ActionResultTypes()
        {            
            var assembliesToSearch = new[] { Assembly.GetExecutingAssembly() }; 

            var actionResultTypes = FindTypesImplementingInterface<IActionResult>(assembliesToSearch);

            return Ok(actionResultTypes.Select(i => i.FullName).ToList());

        }
    }
}
```

```cs
services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
        });
```