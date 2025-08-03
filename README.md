# Edi.Gravatar

[![NuGet](https://img.shields.io/nuget/v/Edi.Gravatar.svg)](https://www.nuget.org/packages/Edi.Gravatar/)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue.svg)](https://dotnet.microsoft.com/)

ASP.NET Core Gravatar Image TagHelper for easily displaying Gravatar profile images in your web applications.

## Installation

Install the package via NuGet Package Manager:

```
dotnet add package Edi.Gravatar
```

Or via Package Manager Console:

```
Install-Package Edi.Gravatar
```

## Setup

Add the TagHelper to your `_ViewImports.cshtml`:

```razor
@addTagHelper *, Edi.Gravatar
```

## Usage

### Basic Usage

```razor
<gravatar email="user@example.com" />
```

### Advanced Usage

```razor
<gravatar email="user@example.com" size="120" alt="User Profile Picture" default-image-url="https://example.com/default-avatar.png" prefer-https="true" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `email` | `string` | `null` | The email address to generate Gravatar for |
| `size` | `int` | `58` | Image size in pixels (1-2048) |
| `alt` | `string` | `"Gravatar image"` | Alt text for the image |
| `default-image-url` | `string` | `""` | URL of default image when Gravatar not found |
| `prefer-https` | `bool` | `true` | Use HTTPS protocol for Gravatar URLs |
| `force-default-image` | `bool` | `false` | Always show default image instead of Gravatar |

## Examples

### Different Sizes

```razor
<!-- Small avatar --> <gravatar email="user@example.com" size="32" />
<!-- Medium avatar --> <gravatar email="user@example.com" size="64" />
<!-- Large avatar --> <gravatar email="user@example.com" size="128" />
```

### With Default Image

```razor
<gravatar email="user@example.com" size="80" default-image-url="https://via.placeholder.com/80" />
```

## Requirements

- .NET 8.0 or .NET 9.0
- ASP.NET Core