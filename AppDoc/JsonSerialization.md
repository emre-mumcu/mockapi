# Default Property Serialization Order

When serializing an object using System.Text.Json or Newtonsoft.Json, both libraries use reflection via the GetProperties method. In this case, the method first returns the properties of the derived class. If using inheritance, it will return the properties of the inherited classes in hierarchical order. 

It should be noted that in .NET 6 and earlier versions, the GetProperties method does not return properties in a particular order, such as alphabetical or declaration order. Your code must not depend on the order in which properties are returned, because that order varies. However, starting with .NET 7, the ordering is deterministic based on the metadata ordering in the assembly. (Microsoft – System.Type.GetProperties)

```cs
public class Person 
{ 
    public int Id { get; set; } 
    public string? Name { get; set; } 
}
public class Student: Person
{
    public int RegistratioNumber { get; set; }
    public double Grade { get; set; }
}
var properties = typeof(Student)
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .ToList();

foreach (var property in properties) Console.WriteLine($"{property.Name} {property.MetadataToken}");
```

## Order Using Property Attribute

To modify the default JSON serialization order of the System.Text.Json library, we can utilize the [JsonPropertyOrder] attribute on class properties that require a different order. It’s worth noting that any property that lacks an order setting will receive a value of zero (0). Hence, if we wish to order a property first by using a positive value, we must assign a higher order to all other serialized properties. Alternatively, we can set the initial property’s order to -1.

To modify the serialization order via the Newtonsoft.Json library, we can utilize the [JsonProperty] attribute decorator from the Newtonsoft.Json namespace and specify the Order as -2. This is necessary because the default value, in this case, is -1.

## Order Using JsonConverter

In System.Text.Json, JsonConverter is a class that can be used to customize the serialization and deserialization of a type. A JsonConverter can be used to control how an object is converted to a JSON string and vice versa. Let’s create a JsonConverter that alphabetically sorts the properties of a class:

```cs
public class MicrosoftOrderedPropertiesConverter<T> : JsonConverter<T>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(T).IsAssignableFrom(typeToConvert);
    }
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        var properties = typeof(T).GetProperties().OrderBy(p => p.Name).ToList();
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(value);
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, options);
        }
        writer.WriteEndObject();
    }
}
```

To demonstrate its use, let’s create anAnimal class and add the MicrosoftOrderedPropertiesConverter as an attribute class:

```cs
[JsonConverter(typeof(MicrosoftOrderedPropertiesConverter<Animal>))]
public class Animal
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}
```

We can also register the converter through JsonSerializerOptions:

```cs
var options = new JsonSerializerOptions
{
    WriteIndented = true
};
options.Converters.Add(new MicrosoftOrderedPropertiesConverter<Animal>());
var json = JsonSerializer.Serialize(animal, options);
Console.WriteLine(json);
```

We can do the same in Newtosoft.Json, but with a few differences. Now, let’s show the same behavior using the JsonConverter from this library:

```cs
public class NewtonsoftOrderedPropertiesConverter<T> : JsonConverter<T>
{
    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (!hasExistingValue)
        {
            existingValue = Activator.CreateInstance<T>();
        }
        serializer.Populate(reader, existingValue!);
        return existingValue;
    }
    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        var properties = typeof(T).GetProperties().OrderBy(p => p.Name);
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(value);
            writer.WritePropertyName(property.Name);
            serializer.Serialize(writer, propertyValue);
        }
        writer.WriteEndObject();
    }
}
```

Now, we need to pass the custom NewtonsoftOrderedPropertiesConverter to the JsonSerializerSettings:

```cs
var json = JsonConvert.SerializeObject(animal,
    Formatting.Indented,
    new NewtonsoftOrderedPropertiesConverter<Animal>()
);
```

## Order Using IJsonTypeInfoResolver

Starting with .NET 7, users can write their own JSON contract resolution logic using implementations of the IJsonTypeInfoResolver interface. Contract resolution performed by the default serializer is exposed through the DefaultJsonTypeInfoResolver class, which implements IJsonTypeInfoResolver.

Let’s create a OrderedPropertiesJsonTypeInfoResolver class that overrides DefaultJsonTypeInfoResolver to sort the properties:

```cs
public class OrderedPropertiesJsonTypeInfoResolver: DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var order = 0;
        JsonTypeInfo typeInfo = base.GetTypeInfo(type, options);
        if (typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            foreach (JsonPropertyInfo property in typeInfo.Properties.OrderBy(a => a.Name))
            {
                property.Order = order++;
            }
        }
        return typeInfo;
    }
}
```

We can use this class by instantiating and setting it to the TypeInfoResolver property of the JsonSerializerOptions class:

```cs
var json = JsonSerializer.Serialize(instance,
    new JsonSerializerOptions 
    { 
        WriteIndented = true, 
        TypeInfoResolver = new OrderedPropertiesJsonTypeInfoResolver() 
    }
);
```

## Order Using IContractResolver

By default, Newtonsoft.Json uses the DefaultContractResolver, which implements the IContractResolver interface and provides a set of default serialization contracts for the most common types. However, we can create our own custom contract resolver by implementing the IContractResolver interface. This approach enables us to define custom serialization contracts tailored to specific types or members.

For our case, we can sort the properties through the override of DefaultContractResolver:

```cs
public class OrderedPropertiesContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);
        return properties.OrderBy(p => p.PropertyName).ToList();
    }
}

// alternative

    public class SortedPropertiesContractResolver : DefaultContractResolver
    {
        // use a static instance for optimal performance
        static SortedPropertiesContractResolver instance;

        static SortedPropertiesContractResolver() { instance = new SortedPropertiesContractResolver(); }

        public static SortedPropertiesContractResolver Instance { get { return instance; } }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            if (properties != null)
                return properties.OrderBy(p => p.UnderlyingName).ToList();
            return properties;
        }
    }
```
And then, we can use it by instantiating it in the ContractResolver property of the JsonSerializerSettings class:

```cs
var json = JsonConvert.SerializeObject(animal,
    Formatting.Indented,
    new JsonSerializerSettings
    {
        ContractResolver = new OrderedPropertiesContractResolver()
    }
);
```

# Reference
https://code-maze.com/csharp-property-ordering-json-serialization/
https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-8.0