# Serilog.Sinks.Http - A Serilog sink sending log events over HTTP

[![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Http.Loki.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Http.Loki/)
[![Documentation](https://img.shields.io/badge/docs-wiki-yellow.svg)](https://github.com/serilog/serilog/wiki)
[![Join the chat at https://gitter.im/serilog/serilog](https://img.shields.io/gitter/room/serilog/serilog.svg)](https://gitter.im/serilog/serilog)
[![Help](https://img.shields.io/badge/stackoverflow-serilog-orange.svg)](http://stackoverflow.com/questions/tagged/serilog)

__Package__ - [Serilog.Sinks.Http.Loki](https://www.nuget.org/packages/serilog.sinks.http.loki)
| __Platforms__ - .NET 4.5/4.6.1, .NET Standard 1.3/2.0/2.1

# Serilog.Sinks.Http.Loki

![.NET Core](https://github.com/ilonze/Serilog.Sinks.Http.Loki/workflows/.NET%205.0/badge.svg?branch=master)

This is a Serilog Sink for Grafana's new [Loki Log Aggregator](https://grafana.com/loki).

What is Loki?

> Loki is a horizontally-scalable, highly-available, multi-tenant log aggregation system inspired by Prometheus. It is designed to be very cost effective and easy to operate, as it does not index the contents of the logs, but rather a set of labels for each log stream.

You can find more information about what Loki is over on [Grafana's website here](https://grafana.com/loki).

![Loki Screenshot](https://raw.githubusercontent.com/ilonze/Serilog.Sinks.Http.Loki/master/assets/screenshot.png)

## Current Features:

- Formats and batches log entries to Loki via HTTP
- Ability to provide global Loki log labels
- Comes baked with an HTTP client, but your own can be provided
- Provides contextual log labels

Coming soon:

- Write logs to disk in the correct format to send via Promtail
- Send logs to Loki via HTTP using Snappy compression

## Installation

The Serilog.Sinks.Http.Loki NuGet [package can be found here](https://www.nuget.org/packages/Serilog.Sinks.Http.Loki/). Alternatively you can install it via one of the following commands below:

NuGet command:
```bash
Install-Package Serilog.Sinks.Http.Loki
```
.NET Core CLI:
```bash
dotnet add package Serilog.Sinks.Http.Loki
```

## Basic Example:

```csharp
// var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
var credentials = new NoAuthCredentials("http://localhost:3100"); // Address to local or remote Loki server

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.HttpLoki(serverUrl: credentials)
        .CreateLogger();

var exception = new {Message = ex.Message, StackTrace = ex.StackTrace};
Log.Error(exception);

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;
Log.Information("Message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

Log.CloseAndFlush();
```

### Adding global labels

Loki indexes and groups log streams using labels, in Serilog.Sinks.Http.Loki you can attach labels to all log entries by passing an implementation `ILogLabelProvider` to the `WriteTo.HttpLoki(..)` configuration method. This is ideal for labels such as instance IDs, environments and application names:

```csharp
public class LogLabelProvider : ILogLabelProvider
{

    public IList<LokiLabel> GetLabels()
    {
        return new List<LokiLabel>
        {
            new LokiLabel { Key = "app", Value = "demoapp" },
            new LokiLabel { Key = "environment", Value = "production" }
        };
    }

}
```
```csharp
var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
var log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.HttpLoki(credentials: credentials, formatProvider: new LogLabelProvider())
        .CreateLogger();
```

### Local, contextual labels

In some occasions you'll want to add context to your log stream within a particular class or method, this can be achieved using contextual labels:

```csharp
using (LogContext.PushProperty("PropertyA", "a"))
{
    log.Warning("Warning with Property PropertyA");
    log.Fatal("Fatal with Property PropertyA");
}
```

### Custom HTTP Client

Serilog.Loki.Sink is built on top of the popular [Serilog.Sinks.Http](https://github.com/FantasticFiasco/serilog-sinks-http) library to post log entries to Loki. With this in mind you can you can extend the default HttpClient (`DefaultLokiHttpClient`), or replace it entirely by implementing `ILokiHttpClient`.

```csharp
// ExampleHttpClient.cs

public class ExampleHttpClient : DefaultLokiHttpClient
{
    public override Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return base.PostAsync(requestUri, content);
    }
}
```
```csharp
// Usage

var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
var log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.HttpLoki(credentials: credentials, httpClient: new ExampleHttpClient(), formatProvider: new LogLabelProvider())
        .CreateLogger();
```



### Configure using appsettings.json
`Serilog.Sinks.Http.Loki` can be configured with `appsettings.json` using `Serilog.Settings.Configuration`.  
It support the following arguments `serverUrl`, `username`, `password`, `credentials`, `batchPostingLimit`, `queueLimit`, `period`, `restrictedToMinimumLevel`, `httpClient`, `outputTemplate`, `formatProvider` and `labelProvider`.  
Not all fields can be used in combination look in [LoggerSinkConfigurationExtensions.cs](src/Serilog.Sinks.Http.Loki/LoggerSinkConfigurationExtensions.cs) for the supported combinations.  
`credentials`, `labelProvider`, `httpClient`, and `formatProvider` are classes and must be specified using the `Namespace.ClassName, Assembly` syntax.
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Verbose"
  },
  "Enrich": [ "FromLogContext" ],
  "WriteTo": [
    {
      "Name": "HttpLoki",
      "Args": {
        "serverUrl": "https://loki:3000",
        "labelProvider": "Namespace.ClassName, Assembly"
      }
    }
  ]
}
```

### Missing a feature or want to contribute?
This package is still in its infancy so if there's anything missing then please feel free to raise a feature request, either that or pull requests are most welcome!
