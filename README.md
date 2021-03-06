# FluentHttpClientMock
## A fluent builder interface to create mock HttpClients in C#

A `System.Net.HttpClient` class cannot be mocked in unit tests as it does not implement an interface that exposes its HTTP operations.

FluentHttpClientMock creates an `HttpClient` instance with set conditions and expectations. If at least once condition is not met, the expectation will not be set. This is in alignment with well-known mocking frameworks such as Rhino Mocks, Moq and NSubstitute.

### Syntax

Instantiate a mocker

```csharp
var mocker = HttpClientMocker.Create();
```

Set HTTP response body based on calling URL

```csharp
mocker.When.AbsoluteUrlIs("https://path.to.url/")
.Then.ResponseShouldBe("Response content");
```

Set HTTP response body based on request body

```csharp
mocker.When.RequestMessageStringIs("Request content")
.Then.ResponseShouldBe("Response content");
```

Chain both conditions
```csharp
mocker.When.RequestMessageStringIs("Request content")
.And.AbsoluteUrlIs("https://path.to.url/")
.Then.ResponseShouldBe("Response content");
```

## Asserting HTTP requests

The `HttpClientMocker` class exposes members that return certain data that was sent through the mocked HTTP client.

`AbsoluteRequestUri` returns the absolute URI that the HTTP client used to make its last HTTP call
`RequestContent` returns the content body that the HTTP client sent last 

## Building the project
The project uses [Cake](https://cakebuild.net/) for its build process. Running `build.ps1` (Remote Signed script) will version and compile the project, run the unit tests and create a NuGet package. If you don't want to package the project at this moment, you can change the final build task on the last line (`RunTarget("Package");`) to "`RunTarget("Package");`".

The mocker is written in .NET Standard 2.0
