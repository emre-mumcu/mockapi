# LINQ Aggregate algorithm

The easiest-to-understand definition of Aggregate is that it performs an operation on each element of the list taking into account the operations that have gone before. That is to say it performs the action on the first and second element and carries the result forward. Then it operates on the previous result and the third element and carries forward. etc.

```cs
// Summing numbers
var nums = new[]{1,2,3,4};
var sum = nums.Aggregate( (a,b) => a + b);
Console.WriteLine(sum); // output: 10 (1+2+3+4)

// join list to a string
var chars = new []{"a","b","c","d"};
var csv = chars.Aggregate( (a,b) => a + ',' + b);
Console.WriteLine(csv); // Output a,b,c,d

// // join list to a string with string builder
var chars = new []{"a","b","c", "d"};
var csv = chars.Aggregate(new StringBuilder(), (a,b) => {
    if(a.Length>0)
        a.Append(",");
    a.Append(b);
    return a;
});
Console.WriteLine(csv);
```

## Linq Aggregate complex types into a string

How could the 'Aggregate' function be used if you wish to aggregate more complex types?

```cs
// You have two options:

// 1) Project to a string and then aggregate:

var values = new[] {
    new { Key = "MyAge", Value = 33.0 },
    new { Key = "MyHeight", Value = 1.75 },
    new { Key = "MyWeight", Value = 90.0 }
};
var res1 = values.Select(x => string.Format("{0}:{1}", x.Key, x.Value))
                .Aggregate((current, next) => current + ", " + next);
Console.WriteLine(res1);

//This has the advantage of using the first string element as the seed (no prepended ", "), but will consume more memory for the strings created in the process.

// 2) Use an aggregate overload that accepts a seed, perhaps a StringBuilder:

var res2 = values.Aggregate(new StringBuilder(),
    (current, next) => current.AppendFormat(", {0}:{1}", next.Key, next.Value),
    sb => sb.Length > 2 ? sb.Remove(0, 2).ToString() : "");
Console.WriteLine(res2);

//The second delegate converts our StringBuilder into a string, using the conditional to trim the starting ", ".
```