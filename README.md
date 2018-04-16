# Utf8Json.Jsonp

Fast JSONP Serializer powered by primitive API of [Utf8Json](https://github.com/neuecc/Utf8Json).

## Install

This library availables for .NET Standard 2.0 only.

Nuget:

```
Install-Package Utf8Json.Jsonp
```

## QuickStart

```csharp
var p = new Person { Age = 99, Name = "foobar" };
var callback = "callbackFunc";

// result: callbackFunc({"Age":99,"Name":"foobar"});

// obj -> byte[]
byte[] bytes = Jsonp.Serialize(callback, p);

// write to stream
using (var stream = new MemoryStream())
{
    Jsonp.Serialize(stream, callback, p);
}
```

## TODO:

* Dynamic Serialization
* AspNetCoreMvcFormatter
