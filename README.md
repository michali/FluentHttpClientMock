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