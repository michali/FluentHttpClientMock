# HttpClientMock
## A fluent builder interface to create mock HttpClients in C#

A `System.Net.HttpClient` class cannot be mocked in unit tests as it does not implement an interface that exposes its HTTP operations. This is a solution that 