# StructLog
 
[![NuGet](https://img.shields.io/nuget/v/StructLog.svg)](https://www.nuget.org/packages/StructLog)
[![Publish to NuGet](https://github.com/simoneM93/StructLog/actions/workflows/publish.yml/badge.svg)](https://github.com/simoneM93/StructLog/actions/workflows/publish.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![GitHub Sponsors](https://img.shields.io/badge/Sponsor-%E2%9D%A4-ea4aaa?logo=github-sponsors)](https://github.com/sponsors/simoneM93)
[![Changelog](https://img.shields.io/badge/Changelog-view-blue)](CHANGELOG.md)
 
A modern structured logging library for .NET with support for custom enrichers, JSON output, and full Dependency Injection integration.
 
> **Why StructLog?**
> Standard `ILogger<T>` lets you log anything, but doesn't enforce a consistent structure.
> StructLog requires every log entry to carry an **EventCode** — a short, queryable identifier
> that makes filtering and alerting in production trivial.
 
---
 
## ✨ Features
 
- 🔧 **Structured JSON output** — every log entry is a consistent, queryable JSON object
- 🏷️ **EventCode** — mandatory code + description on every entry, great for monitoring and alerting
- ⚡ **Custom Enrichers** — inject contextual data (machine name, request ID, tenant, …) via `ILogEnricher`
- 🌐 **DI-ready** — works with `ILogger<T>` and `IOptions<StructLogOptions>` out of the box
- 📝 **Configurable** via `appsettings.json`
- 🖥️ **Multi-host** — Console, Worker Service, and ASP.NET Core apps
 
---
 
## 📋 Requirements
 
| Requirement | Minimum version |
|---|---|
| .NET | 9.0+ |
 
---
 
## 🚀 Installation
 
```bash
dotnet add package StructLog
```
 
---
 
## 🎯 Quick Start
 
### 1. Configure `appsettings.json`
 
```json
{
  "StructLog": {
    "EnableEnrichers": true
  }
}
```
 
### 2. Register services
 
```csharp
// With appsettings.json
builder.Services.AddStructLog(builder.Configuration);
 
// Without appsettings (uses defaults)
builder.Services.AddStructLog();
```
 
### 3. Inject and use
 
```csharp
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IStructLog<UsersController> _logger;
 
    public UsersController(IStructLog<UsersController> logger)
    {
        _logger = logger;
    }
 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await GetUserFromDb(id);
 
        if (user is null)
        {
            _logger.Warning(
                "User not found",
                new EventCode("USR_001", "Get user"),
                new { UserId = id });
 
            return NotFound();
        }
 
        _logger.Info(
            "User retrieved successfully",
            new EventCode("USR_002", "Get user"),
            user);
 
        return Ok(user);
    }
}
```
 
### 4. Example log output
 
```json
{
  "EventCode": "USR_002",
  "EventDescription": "Get user",
  "Message": "User retrieved successfully",
  "Context": {
    "MachineName": "server-01"
  },
  "Data": {
    "Id": 1,
    "Name": "Simone",
    "Surname": "Rossi",
    "Birthday": "1993-05-12T00:00:00"
  }
}
```
 
---
 
## 📚 API Reference
 
### Log levels
 
All methods accept an `IEventCode` and an optional `data` object.
 
```csharp
_logger.Trace("Entering method",   eventCode);
_logger.Debug("Cache miss",        eventCode, new { Key = "users:1" });
_logger.Info("User created",       eventCode, user);
_logger.Warning("Retry attempt",   eventCode, new { Attempt = 3 });
_logger.Error("Payment failed",    eventCode, exception, new { OrderId = 42 });
_logger.Critical("DB unreachable", eventCode, exception);
```
 
### Logging scope
 
Use `BeginScope` to attach contextual data to all logs within a block:
 
```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["RequestId"] = HttpContext.TraceIdentifier,
    ["UserId"]    = currentUserId
}))
{
    _logger.Info("Processing request", new EventCode("REQ_001", "Request"));
}
```
 
---
 
## 🔑 EventCode
 
`EventCode` is the core of StructLog. Every log entry must carry one, which makes your logs
immediately searchable and alertable in tools like Kibana, Grafana, or Azure Monitor.
 
```csharp
// Simple inline usage
var code = new EventCode("PAY_003", "Payment processing");
 
// Recommended: define codes as constants in a dedicated class
public static class LogEvents
{
    public static readonly EventCode UserCreated   = new("USR_001", "User lifecycle");
    public static readonly EventCode UserDeleted   = new("USR_002", "User lifecycle");
    public static readonly EventCode PaymentFailed = new("PAY_001", "Payment processing");
}
 
// Usage
_logger.Error("Payment declined", LogEvents.PaymentFailed, exception, new { OrderId = 99 });
```
 
---
 
## 🌐 Custom Enrichers
 
Enrichers automatically attach contextual data to every log entry.
Implement `ILogEnricher` and register it with DI:
 
```csharp
// 1. Implement the interface
public class MachineNameEnricher : ILogEnricher
{
    public void Enrich(IDictionary<string, object> context)
    {
        context["MachineName"] = Environment.MachineName;
    }
}
 
// 2. Register — all ILogEnricher implementations are picked up automatically
builder.Services.AddSingleton<ILogEnricher, MachineNameEnricher>();
 
// 3. Enable in appsettings.json
// "StructLog": { "EnableEnrichers": true }
```
 
You can register as many enrichers as you need. Common use cases: machine name, environment,
request ID, tenant ID, application version.
 
---
 
## ⚙️ Configuration options
 
| Option | Type | Default | Description |
|---|---|---|---|
| `EnableEnrichers` | `bool` | `true` | Whether enrichers are invoked for each log entry |
 
---
 
## ❤️ Support
 
If you find StructLog useful, consider sponsoring its development on GitHub Sponsors.
Even a small contribution helps keep the project maintained and growing.
 
[![Sponsor simoneM93](https://img.shields.io/badge/Sponsor-%E2%9D%A4-ea4aaa?logo=github-sponsors&style=for-the-badge)](https://github.com/sponsors/simoneM93)
 
---
 
## 📄 License
 
MIT — see [LICENSE](LICENSE) for details.