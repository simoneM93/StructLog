# StructLog

[![NuGet](https://img.shields.io/nuget/v/StructLog.svg)](https://www.nuget.org/packages/StructLog)
[![Publish to NuGet](https://github.com/simoneM93/StructLog/actions/workflows/publish.yml/badge.svg)](https://github.com/simoneM93/StructLog/actions/workflows/publish.yml)

A modern structured logging library for .NET with support for custom enrichers, JSON logging, and full Dependency Injection integration.

## ✨ Features

- 🔧 **Structured logs** with JSON output
- 🌐 **DI-ready**: works with ```ILogger<T>``` and ```IOptions<StructLogOptions>```
- ⚡ **Custom Enrichers** support (```ILogEnricher```)
- 📝 **Configurable options** via ```appsettings.json```
- 🖥️ **Works** in Console, Worker, and ASP.NET Core apps
- 
## 📋 Requirements

| Requirement | Minimum Version |
|-------------|-----------------|
| .NET        | 9.0+            |

## 🚀 Installation
```bash
dotnet add package StructLog
```

## 🎯 Quick Start

### 1. Configure `appsettings.json`
```json
{
  "StructLog": {
    "EnableEnrichers": true
  }
}
```

### 2. Register Services
```csharp
// With config
builder.Services.AddStructLog(builder.Configuration);

// Only defaults (Without appsettings)
builder.Services.AddStructLog();
```

### 3. Exemple Usage
```csharp
[ApiController]
public class UserController : ControllerBase
{
    private readonly IStructLog<UserController> _logger;

    public UsersController(IStructLog<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var user = await GetUserFromId(1);

        if(user == null)
        {
             _logger.Warning("User not found", new EventCode("TEST_001", "Get user event"), new { UserId = 1 });
             return NotFound();
        }
        
        _logger.info("User retrieved successfully", new EventCode("TEST_002", "Get user event"), user);
        
        return Ok(user);
    }
}
```

### 4. Exemple Log Output
```json
{
    "EventId": 0,
    "LogLevel": "Information",
    "Category": "Program",
    "Message": {
        "EventCode": "TEST_002",
        "EventDescription": "Get user event",
        "Message": "User retrieved successfully",
        "Context": {
            "MachineName": "Exemple-Machine-01"
        },
        "Data": {
            "Id": 1,
            "Name": "Exemple",
            "Surname": "Exemple",
            "Birthday": "1990-01-01T00:00:00"
        }
    }
}
```
